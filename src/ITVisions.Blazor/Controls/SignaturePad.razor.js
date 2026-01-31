const pads = new Map();

export function init(canvas, dotNetRef, options) {
if (!canvas) {
  return;
}

const ctx = canvas.getContext("2d");
if (!ctx) {
  return;
}

const state = {
  drawing: false,
  hasInk: false,
  lastX: 0,
  lastY: 0,
  dotNetRef,
  ctx,
  handlers: {},
};

ctx.lineWidth = options?.lineWidth ?? 2;
ctx.strokeStyle = options?.strokeColor ?? "#111";
ctx.lineCap = "round";
ctx.lineJoin = "round";

if (options?.backgroundColor) {
  fillBackground(ctx, canvas, options.backgroundColor);
}

  // Zeichne Wasserzeichen (MUSS nach dem Hintergrund gezeichnet werden!)
  if (options?.showWatermark !== false) {
    drawWatermark(ctx, canvas, options?.watermarkText || "Rechtsverbindliche Unterschrift", options?.watermarkText2 || "");
  }

  const pointerDown = (evt) => {
    if (evt.button !== undefined && evt.button !== 0) {
      return;
    }
    state.drawing = true;
    const pos = getPos(canvas, evt);
    state.lastX = pos.x;
    state.lastY = pos.y;
    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
    canvas.setPointerCapture?.(evt.pointerId);
    evt.preventDefault();
  };

  const pointerMove = (evt) => {
    if (!state.drawing) {
      return;
    }
    const pos = getPos(canvas, evt);
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
    state.hasInk = true;
    state.lastX = pos.x;
    state.lastY = pos.y;
    evt.preventDefault();
  };

  const pointerUp = async (evt) => {
    if (!state.drawing) {
      return;
    }
    state.drawing = false;
    canvas.releasePointerCapture?.(evt.pointerId);
    evt.preventDefault();

    if (state.dotNetRef?.invokeMethodAsync) {
      await state.dotNetRef.invokeMethodAsync("NotifyChanged");
    }
  };

  const pointerLeave = (evt) => {
    if (!state.drawing) {
      return;
    }
    state.drawing = false;
    canvas.releasePointerCapture?.(evt.pointerId);
  };

  state.handlers.pointerDown = pointerDown;
  state.handlers.pointerMove = pointerMove;
  state.handlers.pointerUp = pointerUp;
  state.handlers.pointerLeave = pointerLeave;

  canvas.addEventListener("pointerdown", pointerDown);
  canvas.addEventListener("pointermove", pointerMove);
  canvas.addEventListener("pointerup", pointerUp);
  canvas.addEventListener("pointercancel", pointerLeave);
  canvas.addEventListener("pointerleave", pointerLeave);

  pads.set(canvas, state);

  // Speichere die Optionen im State für spätere Verwendung (z.B. beim Clear)
  state.options = options;
}

export function clear(canvas, backgroundColor) {
  const state = pads.get(canvas);
  if (!state) {
    return;
  }

  const { ctx } = state;
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  if (backgroundColor) {
    fillBackground(ctx, canvas, backgroundColor);
  }
  
  // Wasserzeichen nach dem Löschen erneut zeichnen
  if (state.options?.showWatermark) {
    drawWatermark(ctx, canvas, state.options.watermarkText || "Rechtsverbindliche Unterschrift", state.options.watermarkText2 || "");
  }
  
  state.hasInk = false;
}

export function getDataUrl(canvas) {
  if (!canvas) {
    return null;
  }
  return canvas.toDataURL("image/png");
}

export function isEmpty(canvas) {
  const state = pads.get(canvas);
  if (!state) {
    return true;
  }
  return !state.hasInk;
}

export function dispose(canvas) {
  const state = pads.get(canvas);
  if (!state) {
    return;
  }

  canvas.removeEventListener("pointerdown", state.handlers.pointerDown);
  canvas.removeEventListener("pointermove", state.handlers.pointerMove);
  canvas.removeEventListener("pointerup", state.handlers.pointerUp);
  canvas.removeEventListener("pointercancel", state.handlers.pointerLeave);
  canvas.removeEventListener("pointerleave", state.handlers.pointerLeave);

  if (state.dotNetRef?.dispose) {
    state.dotNetRef.dispose();
  }

  pads.delete(canvas);
}

export function disable(canvas) {
  const state = pads.get(canvas);
  if (!state) {
    return;
  }

  // Entferne alle Event-Listener, um die Interaktion zu deaktivieren
  canvas.removeEventListener("pointerdown", state.handlers.pointerDown);
  canvas.removeEventListener("pointermove", state.handlers.pointerMove);
  canvas.removeEventListener("pointerup", state.handlers.pointerUp);
  canvas.removeEventListener("pointercancel", state.handlers.pointerLeave);
  canvas.removeEventListener("pointerleave", state.handlers.pointerLeave);

  // Markiere als deaktiviert
  state.disabled = true;
}

function getPos(canvas, evt) {
  const rect = canvas.getBoundingClientRect();
  const scaleX = canvas.width / rect.width;
  const scaleY = canvas.height / rect.height;
  return {
    x: (evt.clientX - rect.left) * scaleX,
    y: (evt.clientY - rect.top) * scaleY,
  };
}

function fillBackground(ctx, canvas, color) {
  ctx.save();
  ctx.fillStyle = color;
  ctx.fillRect(0, 0, canvas.width, canvas.height);
  ctx.restore();
}

function drawWatermark(ctx, canvas, text, text2) {
  ctx.save();

  // Zeichne horizontale Linie bei 75% der Höhe
  const lineY = canvas.height * 0.75;
  const lineMargin = 40;
  
  ctx.strokeStyle = "#aaaaaa"; // Dezentes Grau
  ctx.lineWidth = 1;
  ctx.setLineDash([]); // Durchgehende Linie
  ctx.beginPath();
  ctx.moveTo(lineMargin, lineY);
  ctx.lineTo(canvas.width - lineMargin, lineY);
  ctx.stroke();
  
  // Zeichne erste Textzeile unter der Linie
  ctx.fillStyle = "#888888"; // Dezentes Grau für Text
  ctx.font = "11px Arial, sans-serif";
  ctx.textAlign = "center";
  ctx.textBaseline = "top";
  ctx.fillText(text, canvas.width / 2, lineY + 5);
  
  // Zeichne zweite Textzeile, falls vorhanden
  if (text2 && text2.trim() !== "") {
    ctx.fillText(text2, canvas.width / 2, lineY + 18); // 13px Abstand zur ersten Zeile (5 + 11 + 2)
  }
  
  ctx.restore();
}
