let fries1 = document.getElementById('fries1');
let fries2 = document.getElementById('fries2');
let fries3 = document.getElementById('fries3');
let fries4 = document.getElementById('fries4');
let fries5 = document.getElementById('fries5');
let fries6 = document.getElementById('fries6');
let fries7 = document.getElementById('fries7');
let fries8 = document.getElementById('fries8');

fries1.addEventListener('touchstart', handleTouchEvent, true);
fries1.addEventListener('touchmove', handleTouchEvent, true);
fries1.addEventListener('touchend', handleTouchEvent, true);
fries1.addEventListener('touchcancel', handleTouchEvent, true);
fries1.addEventListener('mousemove', handleMouseEvent);
fries2.addEventListener('touchstart', handleTouchEvent, true);
fries2.addEventListener('touchmove', handleTouchEvent, true);
fries2.addEventListener('touchend', handleTouchEvent, true);
fries2.addEventListener('touchcancel', handleTouchEvent, true);
fries2.addEventListener('mousemove', handleMouseEvent);
fries3.addEventListener('touchstart', handleTouchEvent, true);
fries3.addEventListener('touchmove', handleTouchEvent, true);
fries3.addEventListener('touchend', handleTouchEvent, true);
fries3.addEventListener('touchcancel', handleTouchEvent, true);
fries3.addEventListener('mousemove', handleMouseEvent);
fries4.addEventListener('touchstart', handleTouchEvent, true);
fries4.addEventListener('touchmove', handleTouchEvent, true);
fries4.addEventListener('touchend', handleTouchEvent, true);
fries4.addEventListener('touchcancel', handleTouchEvent, true);
fries4.addEventListener('mousemove', handleMouseEvent);


fries5.addEventListener('touchstart', handleTouchEvent, true);
fries5.addEventListener('touchmove', handleTouchEvent, true);
fries5.addEventListener('touchend', handleTouchEvent, true);
fries5.addEventListener('touchcancel', handleTouchEvent, true);
fries5.addEventListener('mousemove', handleMouseEvent);

fries6.addEventListener('touchstart', handleTouchEvent, true);
fries6.addEventListener('touchmove', handleTouchEvent, true);
fries6.addEventListener('touchend', handleTouchEvent, true);
fries6.addEventListener('touchcancel', handleTouchEvent, true);
fries6.addEventListener('mousemove', handleMouseEvent);

fries7.addEventListener('touchstart', handleTouchEvent, true);
fries7.addEventListener('touchmove', handleTouchEvent, true);
fries7.addEventListener('touchend', handleTouchEvent, true);
fries7.addEventListener('touchcancel', handleTouchEvent, true);
fries7.addEventListener('mousemove', handleMouseEvent);

fries8.addEventListener('touchstart', handleTouchEvent, true);
fries8.addEventListener('touchmove', handleTouchEvent, true);
fries8.addEventListener('touchend', handleTouchEvent, true);
fries8.addEventListener('touchcancel', handleTouchEvent, true);
fries8.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let correctAnswer = document.getElementById("correctAnswer");
    let monster1 = document.getElementById('monster1');
    let monster2 = document.getElementById('monster2');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(fries1, monster1) || checkOverlap(fries1, monster2) || checkOverlap(fries2, monster1) || checkOverlap(fries2, monster2) || checkOverlap(fries3, monster1)
        || checkOverlap(fries3, monster2) || checkOverlap(fries4, monster1) || checkOverlap(fries4, monster2) || checkOverlap(fries5, monster1) || checkOverlap(fries5, monster2)
        || checkOverlap(fries6, monster1) || checkOverlap(fries6, monster2) || checkOverlap(fries7, monster1) || checkOverlap(fries7, monster2) || checkOverlap(fries8, monster1)
        || checkOverlap(fries8, monster2)) {
        correctCount++
    }

    if (correctCount === 8) {
        resultText = " WELL DONE!!!"
        correctAnswer.innerHTML = "4"
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
