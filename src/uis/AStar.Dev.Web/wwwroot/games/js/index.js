function addImages(imageName, times, src) {

    for (let i = 1; i <= times; i++) {
        let imageId = imageName + i
        var el = document.getElementById(imageId);

        if (!el) {
            el = document.createElement("div");
            el.id = imageName + i;
            var image = document.createElement("img");
            image.src = './halving/' + imageName + '.png';
            el.appendChild(image);
            src.parentElement.appendChild(el);
        }
    }
}