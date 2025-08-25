function addImages(imageName, times, src) {

    for (let i = 1; i <= times; i++) {
        let imageId = imageName + i
        let el = document.getElementById(imageId);

        if (!el) {
            el = document.createElement("div");
            el.id = imageName + i;
            const image = document.createElement("img");
            image.src = './halving/' + imageName + '.png';
            el.appendChild(image);
            src.parentElement.appendChild(el);
        }
    }
}
