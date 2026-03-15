/** Image zoom */
var ZOOM_OPTIONS = { background: 'rgba(0,0,0,0.7)', margin: 24 };

document.addEventListener('DOMContentLoaded', function () {
    mediumZoom('.page__body img', ZOOM_OPTIONS);

    // Allow plain text links pointing to images to open in medium-zoom overlay
    document.querySelectorAll('.page__body a[href$=".jpg"],.page__body a[href$=".jpeg"],.page__body a[href$=".png"],.page__body a[href$=".webp"],.page__body a[href$=".gif"]').forEach(function (link) {
        if (link.querySelector('img')) return; // already has an img child, skip
        link.addEventListener('click', function (e) {
            e.preventDefault();
            var img = document.createElement('img');
            img.src = link.href;
            img.style.cssText = 'position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);pointer-events:none;visibility:hidden;border-radius:4px;';
            document.body.appendChild(img);
            var z = mediumZoom(img, ZOOM_OPTIONS);
            img.addEventListener('load', function () {
                img.style.visibility = '';
                z.open();
            });
            z.on('closed', function () { z.detach(); img.remove(); });
        });
    });
});
/** End Image zoom */
