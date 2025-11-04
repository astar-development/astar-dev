let cookie1 = document.getElementById('cookie1');
let cookie2 = document.getElementById('cookie2');
let cookie3 = document.getElementById('cookie3');
let cookie4 = document.getElementById('cookie4');

cookie1.addEventListener('touchstart', handleTouchEvent, true);
cookie1.addEventListener('touchmove', handleTouchEvent, true);
cookie1.addEventListener('touchend', handleTouchEvent, true);
cookie1.addEventListener('touchcancel', handleTouchEvent, true);
cookie1.addEventListener('mousemove', handleMouseEvent);
cookie2.addEventListener('touchstart', handleTouchEvent, true);
cookie2.addEventListener('touchmove', handleTouchEvent, true);
cookie2.addEventListener('touchend', handleTouchEvent, true);
cookie2.addEventListener('touchcancel', handleTouchEvent, true);
cookie2.addEventListener('mousemove', handleMouseEvent);
cookie3.addEventListener('touchstart', handleTouchEvent, true);
cookie3.addEventListener('touchmove', handleTouchEvent, true);
cookie3.addEventListener('touchend', handleTouchEvent, true);
cookie3.addEventListener('touchcancel', handleTouchEvent, true);
cookie3.addEventListener('mousemove', handleMouseEvent);
cookie4.addEventListener('touchstart', handleTouchEvent, true);
cookie4.addEventListener('touchmove', handleTouchEvent, true);
cookie4.addEventListener('touchend', handleTouchEvent, true);
cookie4.addEventListener('touchcancel', handleTouchEvent, true);
cookie4.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let correctAnswer = document.getElementById("correctAnswer");
    let monster1 = document.getElementById('monster1');
    let monster2 = document.getElementById('monster2');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(cookie1, monster1) || checkOverlap(cookie1, monster2) || checkOverlap(cookie2, monster1) || checkOverlap(cookie2, monster2) || checkOverlap(cookie3, monster1)
        || checkOverlap(cookie3, monster2) || checkOverlap(cookie4, monster1) || checkOverlap(cookie4, monster2)) {
        correctCount++
    }

    if (correctCount === 4) {
        resultText = " WELL DONE!!!"
        correctAnswer.innerHTML = "2"
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
