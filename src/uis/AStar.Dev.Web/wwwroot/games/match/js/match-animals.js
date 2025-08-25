let fish = document.getElementById('fish');
let mouse = document.getElementById('mouse');
let dog = document.getElementById('dog');
let cat = document.getElementById('cat');

fish.addEventListener('touchstart', handleTouchEvent, true);
fish.addEventListener('touchmove', handleTouchEvent, true);
fish.addEventListener('touchend', handleTouchEvent, true);
fish.addEventListener('touchcancel', handleTouchEvent, true);
fish.addEventListener('mousemove', handleMouseEvent);
mouse.addEventListener('touchstart', handleTouchEvent, true);
mouse.addEventListener('touchmove', handleTouchEvent, true);
mouse.addEventListener('touchend', handleTouchEvent, true);
mouse.addEventListener('touchcancel', handleTouchEvent, true);
mouse.addEventListener('mousemove', handleMouseEvent);
dog.addEventListener('touchstart', handleTouchEvent, true);
dog.addEventListener('touchmove', handleTouchEvent, true);
dog.addEventListener('touchend', handleTouchEvent, true);
dog.addEventListener('touchcancel', handleTouchEvent, true);
dog.addEventListener('mousemove', handleMouseEvent);
cat.addEventListener('touchstart', handleTouchEvent, true);
cat.addEventListener('touchmove', handleTouchEvent, true);
cat.addEventListener('touchend', handleTouchEvent, true);
cat.addEventListener('touchcancel', handleTouchEvent, true);
cat.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let dropBoxFish = document.getElementById('dropBoxFish');
    let dropBoxMouse = document.getElementById('dropBoxMouse');
    let dropBoxCat = document.getElementById('dropBoxCat');
    let dropBoxDog = document.getElementById('dropBoxDog');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(fish, dropBoxFish) || checkOverlap(mouse, dropBoxMouse) || checkOverlap(cat, dropBoxCat) || checkOverlap(dog, dropBoxDog)) {
        correctCount++
    }

    if (correctCount === 4) {
        resultText = " WELL DONE!!!"
    } else {
        resultText = " Sorry, please try again"
    }
    result.innerHTML = resultText
}

function checkOverlap(e, dropzone) {
    if ((e === undefined || e === null) || (dropzone === undefined || dropzone === null)) {
        console.log('something was null, exiting overlay early')
        return false;
    }

    let leftImage = Number(e.style.left.replace('px', ''))
    let leftDropzone = Number(dropzone.style.left.replace('px', ''))
    let leftOk = leftImage >= leftDropzone
    console.log('left Image', leftImage)
    console.log('left dropzone', leftDropzone)
    console.log('left OK', leftOk)

    let rightImage = Number(e.style.left.replace('px', '')) + 64
    let rightDropzone = Number(dropzone.style.left.replace('px', '')) + 295
    let rightOk = rightImage <= rightDropzone
    console.log('right Image', rightImage)
    console.log('right dropzone', rightDropzone)
    console.log('right OK', rightOk)

    let topImage = Number(e.style.top.replace('px', ''))
    let topDropzone = Number(dropzone.style.top.replace('px', ''))
    let topOk = topImage >= topDropzone
    console.log('top Image', topImage)
    console.log('top dropzone', topDropzone)
    console.log('top OK', topOk)

    let bottomImage = Number(e.style.top.replace('px', '')) + 64
    let bottomDropzone = Number(dropzone.style.top.replace('px', '')) + 295
    let bottomOk = bottomImage <= bottomDropzone
    console.log('bottom Image', bottomImage)
    console.log('bottom dropzone', bottomDropzone)
    console.log('bottom OK', bottomOk)

    return leftOk && rightOk && topOk && bottomOk
}

function handleTouchEvent(e) {
    if (e.touches.length === 0) return;
    e.preventDefault();
    e.stopPropagation();
    let touch = e.touches[0];
    e.srcElement.style.left = (touch.pageX - e.srcElement.width / 2) + 'px';
    e.srcElement.style.top = (touch.pageY - e.srcElement.height / 2) + 'px';
}

function handleMouseEvent(e) {
    e.srcElement.style.left = (e.pageX - e.srcElement.width / 2) + 'px';
    e.srcElement.style.top = (e.pageY - e.srcElement.height / 2) + 'px';
}
