document.addEventListener('DOMContentLoaded', () => {
    var linkField = document.getElementById('FileName');
    var imageBrowser = document.getElementById('ImageBrowser');

    fetch('/RuokalistaAdmin/Testingimages')
        .then(response => response.json())
        .then(data => {
            data.forEach(image => {
                var imageLink = document.createElement('div');
                imageLink.textContent = image.name;
                imageLink.classList.add('image-link');
                imageLink.dataset.url = image.url;

                imageLink.addEventListener('mouseover', (event) => {
                    var preview = document.createElement('div');
                    preview.classList.add('image-preview');
                    preview.style.backgroundImage = `url(${event.target.dataset.url})`;
                    document.body.appendChild(preview);

                    imageLink.addEventListener('mousemove', (e) => {
                        preview.style.top = `${e.clientY + 10}px`;
                        preview.style.left = `${e.clientX + 10}px`;
                    });
                });

                imageLink.addEventListener('mouseout', () => {
                    var preview = document.querySelector('.image-preview');
                    if (preview) {
                        preview.remove();
                    }
                });

                imageBrowser.appendChild(imageLink);
            });
        });
});
