import './style.css';

document.querySelector<HTMLDivElement>('#app')!.innerHTML = `
  <header><a class="brand" href="/" aria-label="Ink Lab home"><span class="brand-icon">∿</span> ink<span>lab</span><small>NEURAL WORKSPACE</small></a><span id="connection" class="connection">Connecting…</span></header>
  <main><div class="intro"><div><div class="eyebrow">HANDWRITING RECOGNITION / 01</div><h1>A little ink.<br>A lot of <em>intelligence.</em></h1><p>Draw a digit. See how your neural network thinks.</p></div><div class="architecture"><span>784 <small>INPUTS</small></span><b>→</b><span>128 <small>HIDDEN</small></span><b>→</b><span>10 <small>OUTPUTS</small></span></div></div>
  <div class="workspace"><section class="panel drawing"><div class="panel-title"><h2><span>01</span> Drawing studio</h2><span class="tag">DIGITS 0–9</span></div><div class="canvas-wrap"><canvas id="canvas" width="280" height="280" aria-label="Draw a digit using a mouse, touch, or pen"></canvas><div id="hint">Make your mark<span>Draw one digit in the center</span></div><span class="corner top">+</span><span class="corner bottom">+</span></div><div class="draw-tools"><span>✎ &nbsp; Freehand <small>· 28 × 28 input</small></span><button id="clear" class="text-button">↺ &nbsp; Clear canvas</button></div><button id="recognize" class="primary" disabled>Recognize digit <span>↗</span></button><p class="helper" id="draw-help">Train your network to start recognizing.</p></section>
  <section class="panel results"><div class="panel-title"><h2><span>02</span> Network output</h2><span class="live-dot"></span></div><div class="prediction"><div><div class="eyebrow">PREDICTED DIGIT</div><strong id="digit">—</strong></div><div class="score"><strong id="score">—</strong><span>activation score</span></div></div><div class="distribution"><div class="eyebrow">OUTPUT ACTIVATIONS</div><div id="bars">${Array.from({length:10}, (_, i) => `<div class="bar-row" data-digit="${i}"><span>${i}</span><div class="track"><div class="fill"></div></div><small>—</small></div>`).join('')}</div></div><div class="note"><span>↳</span><p>Each bar is a neuron’s sigmoid activation, not a calibrated probability.</p></div></section>
  <section class="panel training"><div class="panel-title"><h2><span>03</span> Training setup</h2><span class="tag">MNIST</span></div><p class="section-description">Give your network something to learn from.</p><form id="training-form"><label for="folder">Dataset folder</label><input id="folder" placeholder="C:\\datasets\\mnist" required autocomplete="off"/><p class="helper">Local folder containing the four extracted MNIST files.</p><div class="fields"><div><label for="steps">Training steps</label><input id="steps" type="number" value="200000" min="1000" max="2000000" step="1000" required/></div><div><label for="rate">Learning rate</label><input id="rate" type="number" value="0.3" min="0.001" max="1" step="any" required/></div></div><button id="train" class="secondary" disabled>Start training <span>→</span></button></form><div class="training-state"><div><span id="phase">Waiting for connection</span><span id="progress-label">0%</span></div><progress id="progress" value="0" max="100"></progress><p id="training-detail">Your model stays in memory while the API is running.</p></div><div class="accuracy"><span>Test accuracy</span><strong id="accuracy">—</strong></div></section></div>
  <p id="error" role="alert" hidden></p><footer><span><span class="footer-dot">●</span> Built to learn. Made to explore.</span><span>C# neural engine <b> / </b> TypeScript interface</span></footer></main>`;

const el = <T extends HTMLElement = HTMLElement>(id: string) => document.getElementById(id) as T;
type Status = { phase: string; completed: number; total: number; accuracy: number | null; error: string | null };
let ready = false, connected = false, ink = false, predicting = false, drawing = false;
let revision = 0;
const canvas = el<HTMLCanvasElement>('canvas');
const ctx = canvas.getContext('2d')!;
ctx.lineWidth = 20; ctx.lineCap = 'round'; ctx.lineJoin = 'round'; ctx.strokeStyle = '#fff'; ctx.fillStyle = '#fff';
function buttons() { el<HTMLButtonElement>('recognize').disabled = !connected || !ready || !ink || predicting; }
function resetResult() {
  el('digit').textContent = '—'; el('score').textContent = '—';
  document.querySelectorAll<HTMLElement>('.bar-row').forEach(row => { row.classList.remove('winner'); row.querySelector<HTMLElement>('.fill')!.style.width = '0%'; row.querySelector('small')!.textContent = '—'; });
}
function point(e: PointerEvent) { const r = canvas.getBoundingClientRect(); return { x: (e.clientX-r.left)*280/r.width, y: (e.clientY-r.top)*280/r.height }; }
canvas.addEventListener('pointerdown', e => {
  if (e.button !== 0 || drawing) return;
  canvas.setPointerCapture(e.pointerId); drawing = true; ink = true; revision++; resetResult();
  const p = point(e); ctx.beginPath(); ctx.arc(p.x,p.y,10,0,Math.PI*2); ctx.fill(); ctx.beginPath(); ctx.moveTo(p.x,p.y);
  el('hint').hidden = true; buttons();
});
canvas.addEventListener('pointermove', e => { if (drawing) { const p=point(e); ctx.lineTo(p.x,p.y); ctx.stroke(); } });
for (const event of ['pointerup','pointercancel','lostpointercapture']) canvas.addEventListener(event, () => { drawing = false; });
el('clear').addEventListener('click', () => { ctx.clearRect(0,0,280,280); ink=false; drawing=false; revision++; el('hint').hidden=false; resetResult(); buttons(); });
async function api<T>(path: string, body?: unknown): Promise<T> {
  const response = await fetch(`/api/${path}`, { method: body === undefined ? 'GET' : 'POST', headers: { 'Content-Type': 'application/json' }, body: body === undefined ? undefined : JSON.stringify(body), signal: AbortSignal.timeout(15000) });
  if (!response.ok) { const data = await response.json().catch(() => ({})); throw new Error(data.error ?? `Request failed (${response.status})`); }
  return response.status === 202 ? undefined as T : response.json();
}
function error(message: string | null) { el('error').hidden = !message; el('error').textContent = message; }
el('recognize').addEventListener('click', async () => {
  predicting=true; buttons(); error(null); const requestedRevision=revision;
  const small=document.createElement('canvas'); small.width=small.height=28;
  const context=small.getContext('2d')!; context.fillRect(0,0,28,28); context.drawImage(canvas,0,0,28,28);
  const data=context.getImageData(0,0,28,28).data;
  const pixels=Array.from({length:784},(_,i)=>data[i*4]/255);
  try { const result=await api<{digit:number;scores:number[]}>('predict',{pixels});
    if (revision !== requestedRevision) return;
    el('digit').textContent=String(result.digit); el('score').textContent=`${(result.scores[result.digit]*100).toFixed(1)}%`;
    document.querySelectorAll<HTMLElement>('.bar-row').forEach((row,i)=>{ row.classList.toggle('winner',i===result.digit); row.querySelector<HTMLElement>('.fill')!.style.width=`${result.scores[i]*100}%`; row.querySelector('small')!.textContent=`${(result.scores[i]*100).toFixed(1)}%`; });
  } catch(e) { error(e instanceof Error ? e.message : 'Recognition failed.'); } finally { predicting=false; buttons(); }
});
let submitting=false;
el('training-form').addEventListener('submit', async e => {
  e.preventDefault(); submitting=true; el<HTMLButtonElement>('train').disabled=true; error(null);
  try { await api('train',{folder:el<HTMLInputElement>('folder').value.trim(),steps:Number(el<HTMLInputElement>('steps').value),learningRate:Number(el<HTMLInputElement>('rate').value)}); }
  catch(e) { error(e instanceof Error ? e.message : 'Training failed.'); }
  finally { submitting=false; await poll(); }
});
async function poll() {
  try { const status=await api<Status>('status'); connected=true;
    ready=status.phase==='ready'; const busy=['loading','training','evaluating'].includes(status.phase);
    el('connection').textContent='Engine connected'; el('connection').classList.add('online');
    el<HTMLButtonElement>('train').disabled=busy || submitting;
    el('train').textContent=busy ? 'Training in progress…' : 'Start training →';
    const percent=status.total ? Math.floor(status.completed/status.total*100) : 0;
    el<HTMLProgressElement>('progress').value=percent; el('progress-label').textContent=`${percent}%`;
    el('phase').textContent=({idle:'Ready to train',loading:'Loading dataset',training:'Learning patterns',evaluating:'Evaluating test set',ready:'Training complete',error:'Training failed'} as Record<string,string>)[status.phase] ?? status.phase;
    el('training-detail').textContent=status.error ?? (status.total ? `${status.completed.toLocaleString()} / ${status.total.toLocaleString()} steps` : 'Your model stays in memory while the API is running.');
    el('accuracy').textContent=status.accuracy === null ? '—' : `${status.accuracy.toFixed(2)}%`;
    el('draw-help').textContent=ready ? 'Use the full canvas. Keep your digit centered.' : 'Train your network to start recognizing.';
  } catch { connected=false; el('connection').textContent='Engine offline'; el('connection').classList.remove('online'); el<HTMLButtonElement>('train').disabled=true; el('phase').textContent='Start the local API to connect'; }
  buttons();
}
async function watch() { await poll(); window.setTimeout(watch,1500); }
void watch();
