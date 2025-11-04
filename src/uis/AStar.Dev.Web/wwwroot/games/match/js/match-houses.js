let cottage = document.getElementById('cottage');
let detachedHouse = document.getElementById('detached-house');
let terracedHouse = document.getElementById('terracedHouse');
let semiDetached = document.getElementById('semiDetached');

cottage.addEventListener('touchstart', handleTouchEvent, true);
cottage.addEventListener('touchmove', handleTouchEvent, true);
cottage.addEventListener('touchend', handleTouchEvent, true);
cottage.addEventListener('touchcancel', handleTouchEvent, true);
cottage.addEventListener('mousemove', handledetachedHouseEvent);
detachedHouse.addEventListener('touchstart', handleTouchEvent, true);
detachedHouse.addEventListener('touchmove', handleTouchEvent, true);
detachedHouse.addEventListener('touchend', handleTouchEvent, true);
detachedHouse.addEventListener('touchcancel', handleTouchEvent, true);
detachedHouse.addEventListener('mousemove', handledetachedHouseEvent);
terracedHouse.addEventListener('touchstart', handleTouchEvent, true);
terracedHouse.addEventListener('touchmove', handleTouchEvent, true);
terracedHouse.addEventListener('touchend', handleTouchEvent, true);
terracedHouse.addEventListener('touchcancel', handleTouchEvent, true);
terracedHouse.addEventListener('mousemove', handledetachedHouseEvent);
semiDetached.addEventListener('touchstart', handleTouchEvent, true);
semiDetached.addEventListener('touchmove', handleTouchEvent, true);
semiDetached.addEventListener('touchend', handleTouchEvent, true);
semiDetached.addEventListener('touchcancel', handleTouchEvent, true);
semiDetached.addEventListener('mousemove', handledetachedHouseEvent);

function checkResults() {
    let result = document.getElementById("result");
    let dropBoxCottage = document.getElementById('dropBoxCottage');
    let dropBoxDetachedHouse = document.getElementById('dropBoxDetachedHouse');
    let dropBoxSemiDetached = document.getElementById('dropBoxSemiDetached');
    let dropBoxTerracedHouse = document.getElementById('dropBoxTerracedHouse');

    let resultText = ""
    let correctCount = 0

    if (checkOverlap(cottage, dropBoxCottage)) {
        correctCount++
    }
    if (checkOverlap(detachedHouse, dropBoxDetachedHouse)) {
        correctCount++
    }
    if (checkOverlap(semiDetached, dropBoxSemiDetached)) {
        correctCount++
    }
    if (checkOverlap(terracedHouse, dropBoxTerracedHouse)) {
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

function handledetachedHouseEvent(e) {
    e.srcElement.style.left = (e.pageX - e.srcElement.width / 2) + 'px';
    e.srcElement.style.top = (e.pageY - e.srcElement.height / 2) + 'px';
}