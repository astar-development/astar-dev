let icecream1 = document.getElementById('icecream1');
let icecream2 = document.getElementById('icecream2');
let icecream3 = document.getElementById('icecream3');
let icecream4 = document.getElementById('icecream4');
let icecream5 = document.getElementById('icecream5');
let icecream6 = document.getElementById('icecream6');

icecream1.addEventListener('touchstart', handleTouchEvent, true);
icecream1.addEventListener('touchmove', handleTouchEvent, true);
icecream1.addEventListener('touchend', handleTouchEvent, true);
icecream1.addEventListener('touchcancel', handleTouchEvent, true);
icecream1.addEventListener('mousemove', handleMouseEvent);
icecream2.addEventListener('touchstart', handleTouchEvent, true);
icecream2.addEventListener('touchmove', handleTouchEvent, true);
icecream2.addEventListener('touchend', handleTouchEvent, true);
icecream2.addEventListener('touchcancel', handleTouchEvent, true);
icecream2.addEventListener('mousemove', handleMouseEvent);
icecream3.addEventListener('touchstart', handleTouchEvent, true);
icecream3.addEventListener('touchmove', handleTouchEvent, true);
icecream3.addEventListener('touchend', handleTouchEvent, true);
icecream3.addEventListener('touchcancel', handleTouchEvent, true);
icecream3.addEventListener('mousemove', handleMouseEvent);
icecream4.addEventListener('touchstart', handleTouchEvent, true);
icecream4.addEventListener('touchmove', handleTouchEvent, true);
icecream4.addEventListener('touchend', handleTouchEvent, true);
icecream4.addEventListener('touchcancel', handleTouchEvent, true);
icecream4.addEventListener('mousemove', handleMouseEvent);
icecream5.addEventListener('touchstart', handleTouchEvent, true);
icecream5.addEventListener('touchmove', handleTouchEvent, true);
icecream5.addEventListener('touchend', handleTouchEvent, true);
icecream5.addEventListener('touchcancel', handleTouchEvent, true);
icecream5.addEventListener('mousemove', handleMouseEvent);
icecream6.addEventListener('touchstart', handleTouchEvent, true);
icecream6.addEventListener('touchmove', handleTouchEvent, true);
icecream6.addEventListener('touchend', handleTouchEvent, true);
icecream6.addEventListener('touchcancel', handleTouchEvent, true);
icecream6.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let correctAnswer = document.getElementById("correctAnswer");
    let monster1 = document.getElementById('monster1');
    let monster2 = document.getElementById('monster2');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(icecream1, monster1) || checkOverlap(icecream1, monster2) || checkOverlap(icecream2, monster1) || checkOverlap(icecream2, monster2) || checkOverlap(icecream3, monster1)
        || checkOverlap(icecream3, monster2) || checkOverlap(icecream4, monster1) || checkOverlap(icecream4, monster2) || checkOverlap(icecream5, monster1) || checkOverlap(icecream5, monster2)
        || checkOverlap(icecream6, monster1) || checkOverlap(icecream6, monster2)) {
        correctCount++
    }

    if (correctCount === 6) {
        resultText = " WELL DONE!!!"
        correctAnswer.innerHTML = "3"
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

    let rightImage = Number(e.style.left.replace('px', '')) + 64
    let rightDropzone = Number(dropzone.style.left.replace('px', '')) + 250
    let rightOk = rightImage <= rightDropzone

    let topImage = Number(e.style.top.replace('px', ''))
    let topDropzone = Number(dropzone.style.top.replace('px', ''))
    let topOk = topImage >= topDropzone

    let bottomImage = Number(e.style.top.replace('px', '')) + 64
    let bottomDropzone = Number(dropzone.style.top.replace('px', '')) + 150
    let bottomOk = bottomImage <= bottomDropzone

    let overlay = leftOk && rightOk && topOk && bottomOk

    return overlay
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
