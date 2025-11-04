let burger1 = document.getElementById('burger1');
let burger2 = document.getElementById('burger2');
let burger3 = document.getElementById('burger3');
let burger4 = document.getElementById('burger4');
let burger5 = document.getElementById('burger5');
let burger6 = document.getElementById('burger6');
let burger7 = document.getElementById('burger7');
let burger8 = document.getElementById('burger8');
let burger9 = document.getElementById('burger9');
let burger10 = document.getElementById('burger10');

burger1.addEventListener('touchstart', handleTouchEvent, true);
burger1.addEventListener('touchmove', handleTouchEvent, true);
burger1.addEventListener('touchend', handleTouchEvent, true);
burger1.addEventListener('touchcancel', handleTouchEvent, true);
burger1.addEventListener('mousemove', handleMouseEvent);
burger2.addEventListener('touchstart', handleTouchEvent, true);
burger2.addEventListener('touchmove', handleTouchEvent, true);
burger2.addEventListener('touchend', handleTouchEvent, true);
burger2.addEventListener('touchcancel', handleTouchEvent, true);
burger2.addEventListener('mousemove', handleMouseEvent);
burger3.addEventListener('touchstart', handleTouchEvent, true);
burger3.addEventListener('touchmove', handleTouchEvent, true);
burger3.addEventListener('touchend', handleTouchEvent, true);
burger3.addEventListener('touchcancel', handleTouchEvent, true);
burger3.addEventListener('mousemove', handleMouseEvent);
burger4.addEventListener('touchstart', handleTouchEvent, true);
burger4.addEventListener('touchmove', handleTouchEvent, true);
burger4.addEventListener('touchend', handleTouchEvent, true);
burger4.addEventListener('touchcancel', handleTouchEvent, true);
burger4.addEventListener('mousemove', handleMouseEvent);


burger5.addEventListener('touchstart', handleTouchEvent, true);
burger5.addEventListener('touchmove', handleTouchEvent, true);
burger5.addEventListener('touchend', handleTouchEvent, true);
burger5.addEventListener('touchcancel', handleTouchEvent, true);
burger5.addEventListener('mousemove', handleMouseEvent);

burger6.addEventListener('touchstart', handleTouchEvent, true);
burger6.addEventListener('touchmove', handleTouchEvent, true);
burger6.addEventListener('touchend', handleTouchEvent, true);
burger6.addEventListener('touchcancel', handleTouchEvent, true);
burger6.addEventListener('mousemove', handleMouseEvent);

burger7.addEventListener('touchstart', handleTouchEvent, true);
burger7.addEventListener('touchmove', handleTouchEvent, true);
burger7.addEventListener('touchend', handleTouchEvent, true);
burger7.addEventListener('touchcancel', handleTouchEvent, true);
burger7.addEventListener('mousemove', handleMouseEvent);

burger8.addEventListener('touchstart', handleTouchEvent, true);
burger8.addEventListener('touchmove', handleTouchEvent, true);
burger8.addEventListener('touchend', handleTouchEvent, true);
burger8.addEventListener('touchcancel', handleTouchEvent, true);
burger8.addEventListener('mousemove', handleMouseEvent);

burger9.addEventListener('touchstart', handleTouchEvent, true);
burger9.addEventListener('touchmove', handleTouchEvent, true);
burger9.addEventListener('touchend', handleTouchEvent, true);
burger9.addEventListener('touchcancel', handleTouchEvent, true);
burger9.addEventListener('mousemove', handleMouseEvent);

burger10.addEventListener('touchstart', handleTouchEvent, true);
burger10.addEventListener('touchmove', handleTouchEvent, true);
burger10.addEventListener('touchend', handleTouchEvent, true);
burger10.addEventListener('touchcancel', handleTouchEvent, true);
burger10.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let correctAnswer = document.getElementById("correctAnswer");
    let monster1 = document.getElementById('monster1');
    let monster2 = document.getElementById('monster2');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(burger1, monster1) || checkOverlap(burger1, monster2) || checkOverlap(burger2, monster1) || checkOverlap(burger2, monster2) || checkOverlap(burger3, monster1)
        || checkOverlap(burger3, monster2) || checkOverlap(burger4, monster1) || checkOverlap(burger4, monster2) || checkOverlap(burger5, monster1) || checkOverlap(burger5, monster2)
        || checkOverlap(burger6, monster1) || checkOverlap(burger6, monster2) || checkOverlap(burger7, monster1) || checkOverlap(burger7, monster2) || checkOverlap(burger8, monster1)
        || checkOverlap(burger8, monster2) || checkOverlap(burger9, monster1) || checkOverlap(burger9, monster2) || checkOverlap(burger10, monster1) || checkOverlap(burger10, monster2)) {
        correctCount++
    }

    if (correctCount === 10) {
        resultText = " WELL DONE!!!"
        correctAnswer.innerHTML = "5"
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
    console.log('leftImage-' + e.id + ': ' + leftImage + ' >= leftDropzone-' + dropzone.id + ': ' + leftDropzone + ' = ' + leftOk)
    console.log('rightImage-' + e.id + ': ' + rightImage + ' <= rightDropzone-' + dropzone.id + ': ' + rightDropzone + ' = ' + rightOk)
    console.log('topImage-' + e.id + ': ' + topImage + ' >= topDropzone-' + dropzone.id + ': ' + topDropzone + ' = ' + topOk)
    console.log('bottomImage-' + e.id + ': ' + bottomImage + ' <= bottomDropzone-' + dropzone.id + ': ' + bottomDropzone + ' = ' + bottomOk)
    console.log('-----------------------------------------------------------------------')
    console.log('overlay: ' + overlay)
    console.log('=======================================================================')
    console.log(' ')

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
