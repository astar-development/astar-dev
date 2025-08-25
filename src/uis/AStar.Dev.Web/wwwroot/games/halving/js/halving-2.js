let cupcake1 = document.getElementById('cupcake1');
let cupcake2 = document.getElementById('cupcake2');

cupcake1.addEventListener('touchstart', handleTouchEvent, true);
cupcake1.addEventListener('touchmove', handleTouchEvent, true);
cupcake1.addEventListener('touchend', handleTouchEvent, true);
cupcake1.addEventListener('touchcancel', handleTouchEvent, true);
cupcake1.addEventListener('mousemove', handleMouseEvent);
cupcake2.addEventListener('touchstart', handleTouchEvent, true);
cupcake2.addEventListener('touchmove', handleTouchEvent, true);
cupcake2.addEventListener('touchend', handleTouchEvent, true);
cupcake2.addEventListener('touchcancel', handleTouchEvent, true);
cupcake2.addEventListener('mousemove', handleMouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let correctAnswer = document.getElementById("correctAnswer");
    let monster1 = document.getElementById('monster1');
    let monster2 = document.getElementById('monster2');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(cupcake1, monster1) || checkOverlap(cupcake1, monster2) || checkOverlap(cupcake2, monster1) || checkOverlap(cupcake2, monster2)) {
        correctCount++
    }

    if (correctCount === 2) {
        resultText = " WELL DONE!!!"
        correctAnswer.innerHTML = "1"
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
