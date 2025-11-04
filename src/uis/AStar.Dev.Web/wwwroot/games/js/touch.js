import {handleMouseEvent, handleTouchEvent} from './touch-events.js';

let ship = document.getElementById('ship');
let ship2 = document.getElementById('ship2');

ship.addEventListener('touchstart', handleTouchEvent, true);
ship.addEventListener('touchmove', handleTouchEvent, true);
ship.addEventListener('touchend', handleTouchEvent, true);
ship.addEventListener('touchcancel', handleTouchEvent, true);
ship.addEventListener('mousemove', handleMouseEvent);
ship2.addEventListener('touchstart', handleTouchEvent, true);
ship2.addEventListener('touchmove', handleTouchEvent, true);
ship2.addEventListener('touchend', handleTouchEvent, true);
ship2.addEventListener('touchcancel', handleTouchEvent, true);
ship2.addEventListener('mousemove', handleMouseEvent);
