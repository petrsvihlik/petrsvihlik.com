/* ============================================================
   petrsvihlik.com — shared behaviour
   (ported from Claude Design handoff)
   ============================================================ */
(function () {
  'use strict';

  var root = document.documentElement;
  var reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

  /* ---------- theme + accent persistence ---------- */
  var THEME_KEY = 'ps-theme';
  var ACCENT_KEY = 'ps-accent';

  var theme = localStorage.getItem(THEME_KEY) || 'dark';
  var accent = localStorage.getItem(ACCENT_KEY) || 'cyan';
  root.setAttribute('data-theme', theme);
  root.setAttribute('data-accent', accent);

  function syncThemeBtn() {
    var b = document.querySelector('[data-theme-toggle]');
    if (b) b.textContent = theme === 'dark' ? 'sun' : 'moon';
  }
  function syncSwatches() {
    document.querySelectorAll('.sw').forEach(function (s) {
      s.setAttribute('aria-pressed', s.dataset.accent === accent ? 'true' : 'false');
    });
  }

  document.addEventListener('click', function (e) {
    var t = e.target.closest('[data-theme-toggle]');
    if (t) {
      theme = (theme === 'dark') ? 'light' : 'dark';
      root.setAttribute('data-theme', theme);
      localStorage.setItem(THEME_KEY, theme);
      syncThemeBtn();
      return;
    }
    var sw = e.target.closest('.sw');
    if (sw) {
      accent = sw.dataset.accent;
      root.setAttribute('data-accent', accent);
      localStorage.setItem(ACCENT_KEY, accent);
      syncSwatches();
    }
  });

  /* ---------- avatar pixel-resolve (canvas) ---------- */
  function bootAvatar() {
    var cv = document.querySelector('canvas.avatar');
    if (!cv) return;
    var ctx = cv.getContext('2d');
    var W = cv.width, H = cv.height;
    var img = new Image();
    var ready = false;
    var raf = null;

    function draw(res) {
      res = Math.max(2, Math.min(W, Math.round(res)));
      ctx.imageSmoothingEnabled = false;
      ctx.clearRect(0, 0, W, H);
      // downscale into a corner, then scale that block back up = pixelation
      ctx.drawImage(img, 0, 0, res, res);
      ctx.drawImage(cv, 0, 0, res, res, 0, 0, W, H);
    }

    function animate(from, to, dur, done) {
      if (raf) cancelAnimationFrame(raf);
      if (reduce) { draw(to); if (done) done(); return; }
      var t0 = performance.now();
      (function step(now) {
        var p = Math.min(1, (now - t0) / dur);
        var e = 1 - Math.pow(1 - p, 3);           // ease-out
        // quantise to chunky steps so it reads as pixels snapping in
        var res = from + (to - from) * e;
        draw(Math.round(res / 4) * 4);
        if (p < 1) { raf = requestAnimationFrame(step); }
        else { draw(to); if (done) done(); }
      })(t0);
    }

    img.onload = function () {
      ready = true;
      draw(4);
      // resolve in
      setTimeout(function () { animate(4, W, 850); }, 120);
    };
    img.onerror = function () {
      ctx.fillStyle = '#11151c'; ctx.fillRect(0, 0, W, H);
      ctx.fillStyle = '#4fd6e0'; ctx.font = '10px monospace';
      ctx.fillText('no img', 8, 60);
    };
    // real headshot, served from the site's own assets (same-origin)
    img.src = (cv.dataset.src || '/assets/img/avatar.jpg');

    // re-pixelate on hover, resolve back on leave
    cv.addEventListener('mouseenter', function () { if (ready) animate(W, 7, 260); });
    cv.addEventListener('mouseleave', function () { if (ready) animate(7, W, 420); });
  }

  /* ---------- grep filter for article log ----------
     Searches the FULL archive (title · category · date · description) when an
     embedded index is present, so e.g. typing "technology" matches every post
     in that category — not just the ones on the current page. Falls back to
     filtering the on-page rows when no index is available. */
  function initFilter() {
    var input = document.querySelector('[data-grep]');
    if (!input) return;
    var log = document.querySelector('.log');
    var empty = log ? log.querySelector('.empty') : null;
    var countEl = document.querySelector('[data-count]');
    var pager = document.querySelector('.pager');
    var serverRows = Array.prototype.slice.call(document.querySelectorAll('.log .row'));

    var fullIndex = null;
    var idxEl = document.querySelector('script[data-article-index]');
    if (idxEl) {
      try { fullIndex = JSON.parse(idxEl.textContent); } catch (e) { fullIndex = null; }
    }

    var searchRows = [];
    function clearSearchRows() {
      searchRows.forEach(function (r) { r.remove(); });
      searchRows = [];
    }

    function hay(it) {
      return (it.date + ' ' + it.cat + ' ' + it.title + ' ' + it.desc).toLowerCase();
    }

    function buildRow(it) {
      var a = document.createElement('a');
      a.className = 'row';
      a.href = it.url;
      if (it.ext) { a.target = '_blank'; a.rel = 'noopener'; }
      a.setAttribute('data-search', hay(it));

      var date = document.createElement('span'); date.className = 'date'; date.textContent = it.date;
      var cat = document.createElement('span'); cat.className = 'cat'; cat.textContent = it.cat;
      var title = document.createElement('span'); title.className = 'title';
      title.setAttribute('data-text', it.title);
      title.appendChild(document.createTextNode(it.title));
      if (it.desc) {
        var desc = document.createElement('span'); desc.className = 'desc'; desc.textContent = it.desc;
        title.appendChild(desc);
      }
      var go = document.createElement('span'); go.className = 'go'; go.textContent = 'cat →';

      a.appendChild(date); a.appendChild(cat); a.appendChild(title); a.appendChild(go);
      return a;
    }

    function setCount(n) {
      if (countEl) countEl.textContent = n + (n === 1 ? ' entry' : ' entries');
    }

    function showServer() {
      clearSearchRows();
      serverRows.forEach(function (r) { r.style.display = ''; });
      if (pager) pager.style.display = '';
      if (empty) empty.style.display = 'none';
      setCount(serverRows.length);
    }

    function apply() {
      var q = input.value.trim().toLowerCase();
      if (!q) { showServer(); return; }

      if (fullIndex) {
        serverRows.forEach(function (r) { r.style.display = 'none'; });
        if (pager) pager.style.display = 'none';
        clearSearchRows();
        var matches = fullIndex.filter(function (it) { return hay(it).indexOf(q) !== -1; });
        var frag = document.createDocumentFragment();
        matches.forEach(function (it) {
          var row = buildRow(it);
          searchRows.push(row);
          frag.appendChild(row);
        });
        var anchor = serverRows[0] || empty;
        if (anchor) { log.insertBefore(frag, anchor); } else { log.appendChild(frag); }
        if (empty) empty.style.display = matches.length ? 'none' : '';
        setCount(matches.length);
      } else {
        var shown = 0;
        serverRows.forEach(function (r) {
          var ok = (r.dataset.search || r.textContent).toLowerCase().indexOf(q) !== -1;
          r.style.display = ok ? '' : 'none';
          if (ok) shown++;
        });
        if (empty) empty.style.display = shown ? 'none' : '';
        setCount(shown);
      }
    }
    input.addEventListener('input', apply);

    document.addEventListener('keydown', function (e) {
      if (e.key === '/' && document.activeElement !== input) {
        e.preventDefault(); input.focus();
      } else if (e.key === 'Escape' && document.activeElement === input) {
        input.value = ''; apply(); input.blur();
      }
    });
  }

  /* ---------- custom image lightbox ----------
     Replaces medium-zoom. Opens both embedded <img> in content and plain text
     links that point to an image. Click backdrop / ✕ / Esc to close. */
  function initLightbox() {
    var scopes = document.querySelectorAll('.article-single__body, .prose');
    if (!scopes.length) return;
    var IMG_RE = /\.(jpe?g|png|gif|webp|avif|bmp|svg)(\?|#|$)/i;

    var overlay, oImg, oCap;
    function onKey(e) { if (e.key === 'Escape') close(); }
    function build() {
      overlay = document.createElement('div');
      overlay.className = 'lightbox';
      overlay.setAttribute('role', 'dialog');
      overlay.setAttribute('aria-modal', 'true');
      overlay.innerHTML =
        '<button class="lightbox__close" type="button" aria-label="close">✕</button>' +
        '<figure class="lightbox__fig">' +
          '<img class="lightbox__img" alt="">' +
          '<figcaption class="lightbox__cap"></figcaption>' +
        '</figure>';
      oImg = overlay.querySelector('.lightbox__img');
      oCap = overlay.querySelector('.lightbox__cap');
      document.body.appendChild(overlay);
      overlay.addEventListener('click', function (e) {
        if (e.target === overlay || e.target.closest('.lightbox__close')) close();
      });
    }
    function open(src, caption) {
      if (!overlay) build();
      oImg.src = src;
      oImg.alt = caption || '';
      oCap.textContent = caption || '';
      overlay.classList.add('is-open');
      root.style.overflow = 'hidden';
      document.addEventListener('keydown', onKey);
    }
    function close() {
      if (!overlay) return;
      overlay.classList.remove('is-open');
      root.style.overflow = '';
      document.removeEventListener('keydown', onKey);
    }

    scopes.forEach(function (scope) {
      scope.querySelectorAll('img').forEach(function (img) {
        var a = img.closest('a');
        // image wrapped in a non-image link → leave the link alone
        if (a && !IMG_RE.test(a.getAttribute('href') || '')) return;
        img.classList.add('zoomable');
        img.addEventListener('click', function (e) {
          e.preventDefault();
          var src = (a && IMG_RE.test(a.getAttribute('href') || '')) ? a.href : (img.currentSrc || img.src);
          open(src, img.alt || (a ? a.textContent.trim() : ''));
        });
      });
      scope.querySelectorAll('a').forEach(function (a) {
        var href = a.getAttribute('href') || '';
        if (!IMG_RE.test(href) || a.querySelector('img')) return; // text-only image links
        a.classList.add('zoomable');
        a.addEventListener('click', function (e) {
          e.preventDefault();
          open(a.href, a.textContent.trim());
        });
      });
    });
  }

  /* ---------- live GitHub star counts for project tiles ---------- */
  function initStars() {
    var els = document.querySelectorAll('.stars[data-repo]');
    if (!els.length) return;
    function fmt(n) {
      return n >= 1000 ? (n / 1000).toFixed(n >= 10000 ? 0 : 1).replace(/\.0$/, '') + 'k' : String(n);
    }
    els.forEach(function (el) {
      var repo = el.getAttribute('data-repo');
      var out = el.querySelector('.stars-count');
      if (!repo || !out) return;
      fetch('https://api.github.com/repos/' + repo, { headers: { 'Accept': 'application/vnd.github+json' } })
        .then(function (r) { return r.ok ? r.json() : null; })
        .then(function (d) {
          if (d && typeof d.stargazers_count === 'number') {
            out.textContent = fmt(d.stargazers_count);
            el.setAttribute('title', d.stargazers_count.toLocaleString() + ' GitHub stars');
          } else {
            el.style.display = 'none';
          }
        })
        .catch(function () { el.style.display = 'none'; });
    });
  }

  /* ---------- ascii / dither background ---------- */
  function initBg() {
    var c = document.getElementById('bg-canvas');
    if (!c) return;
    var ctx = c.getContext('2d');
    var chars = '01:;<>+=*/\\|[]{}#.-_'.split('');
    var cell = 18, cols, rowsN, grid, dpr;

    function size() {
      dpr = Math.min(window.devicePixelRatio || 1, 2);
      c.width = Math.floor(innerWidth * dpr);
      c.height = Math.floor(innerHeight * dpr);
      c.style.width = innerWidth + 'px';
      c.style.height = innerHeight + 'px';
      cols = Math.ceil(innerWidth / cell) + 1;
      rowsN = Math.ceil(innerHeight / cell) + 1;
      grid = new Array(cols * rowsN);
      for (var i = 0; i < grid.length; i++) {
        grid[i] = {
          ch: chars[(Math.random() * chars.length) | 0],
          a: Math.random() * 0.4
        };
      }
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
      ctx.font = '12px JetBrains Mono, monospace';
      ctx.textBaseline = 'top';
    }

    function color() {
      var s = getComputedStyle(root);
      var fg = s.getPropertyValue('--fg-faint').trim() || '#444';
      var ac = s.getPropertyValue('--accent').trim() || '#4fd6e0';
      return { fg: fg, ac: ac };
    }

    var t = 0, cols2 = color();
    function frame() {
      t++;
      if (t % 3 === 0) cols2 = color();
      ctx.clearRect(0, 0, innerWidth, innerHeight);
      for (var y = 0; y < rowsN; y++) {
        for (var x = 0; x < cols; x++) {
          var g = grid[y * cols + x];
          if (Math.random() < 0.012) {
            g.ch = chars[(Math.random() * chars.length) | 0];
          }
          // slow pulse of alpha
          var pulse = 0.06 + 0.05 * Math.sin((t * 0.012) + (x * 0.4) + (y * 0.3));
          var aval = g.a * pulse;
          // occasional accent glint
          var isAccent = ((x * 7 + y * 13 + (t >> 4)) % 211) === 0;
          ctx.fillStyle = hexA(isAccent ? cols2.ac : cols2.fg, isAccent ? 0.5 : aval);
          ctx.fillText(g.ch, x * cell, y * cell);
        }
      }
      raf = requestAnimationFrame(frame);
    }

    function hexA(hex, a) {
      hex = hex.replace('#', '');
      if (hex.length === 3) hex = hex.replace(/(.)/g, '$1$1');
      var n = parseInt(hex, 16);
      if (isNaN(n) || hex.length < 6) return 'rgba(120,120,120,' + a + ')';
      return 'rgba(' + ((n >> 16) & 255) + ',' + ((n >> 8) & 255) + ',' + (n & 255) + ',' + a + ')';
    }

    var raf;
    size();
    if (reduce) {
      // static single paint
      cols2 = color();
      for (var y = 0; y < rowsN; y++)
        for (var x = 0; x < cols; x++) {
          ctx.fillStyle = hexA(cols2.fg, grid[y * cols + x].a * 0.08);
          ctx.fillText(grid[y * cols + x].ch, x * cell, y * cell);
        }
    } else {
      raf = requestAnimationFrame(frame);
    }

    var rt;
    addEventListener('resize', function () {
      clearTimeout(rt);
      rt = setTimeout(size, 150);
    });
    // pause when tab hidden
    document.addEventListener('visibilitychange', function () {
      if (document.hidden) { cancelAnimationFrame(raf); }
      else if (!reduce) { raf = requestAnimationFrame(frame); }
    });
  }

  /* ---------- go ---------- */
  function init() {
    syncThemeBtn();
    syncSwatches();
    bootAvatar();
    initFilter();
    initLightbox();
    initStars();
    initBg();
  }
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else { init(); }
})();
