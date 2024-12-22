function bglisting(host) {
    const monthSelector = document.getElementById('monthSelector');
    const imageList = document.getElementById('imageList');

    // Function to fetch and display images for a selected month
    function fetchImages(monthIndex) {
        // Clear the existing images
        imageList.innerHTML = '';

        fetch(`${host}/api/v1/GetPictureListing?month=${monthIndex}`)
            .then(response => response.json())
            .then(data => {
                data.forEach(image => {
                    // Create a container for each image
                    const imageLink = document.createElement('div');
                    imageLink.classList.add('image-link');

                    // Create an image element
                    const img = document.createElement('img');
                    img.src = `${host}/api/v1/GetPicturePreview?filename=${image}`;
                    img.alt = image;

                    // Add hover preview functionality
                    imageLink.addEventListener('mouseover', (event) => {
                        const preview = document.createElement('div');
                        preview.classList.add('image-preview');
                        preview.style.backgroundImage = `url(${img.src})`;
                        document.body.appendChild(preview);

                        imageLink.addEventListener('mousemove', (e) => {
                            preview.style.top = `${e.clientY + 10}px`;
                            preview.style.left = `${e.clientX + 10}px`;
                        });
                    });

                    imageLink.addEventListener('mouseout', () => {
                        const preview = document.querySelector('.image-preview');
                        if (preview) {
                            preview.remove();
                        }
                    });

                    // Append the image to the link container
                    imageLink.appendChild(img);
                    // Append the container to the image list
                    imageList.appendChild(imageLink);
                });
            })
            .catch(error => console.error('Error fetching images:', error));
    }

    // Initialize with the default month
    fetchImages(monthSelector.selectedIndex);

    // Update images when the month changes
    monthSelector.addEventListener('change', () => {
        fetchImages(monthSelector.selectedIndex);
    });
}
