// Gera os PDFs da documentação a partir dos arquivos .md desta pasta.
//
// Uso (na pasta docs):
//   node _build/gerar-pdf.mjs             um PDF por documento, na pasta docs/pdf/
//   node _build/gerar-pdf.mjs --completo  também gera Ampara-Documentacao.pdf, com tudo em um só arquivo
//
// Requisitos: Node.js, Google Chrome e acesso à internet (marked e mermaid vêm de uma CDN).
// Variável opcional: CHROME_PATH com o caminho do chrome.exe.

import { mkdirSync, mkdtempSync, readFileSync, writeFileSync } from 'node:fs';
import { execFileSync } from 'node:child_process';
import { tmpdir } from 'node:os';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath, pathToFileURL } from 'node:url';

const aqui = dirname(fileURLToPath(import.meta.url));
const docs = resolve(aqui, '..');
const pastaPdf = join(docs, 'pdf');
const pastaTemporaria = mkdtempSync(join(tmpdir(), 'ampara-docs-'));
const chrome = process.env.CHROME_PATH ?? 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe';
const gerarCompleto = process.argv.includes('--completo');

// [número, arquivo .md, nome do PDF, título curto]
const documentos = [
  ['00', 'README.md', '00-Indice.pdf', 'Índice'],
  ['01', '01-visao-e-escopo.md', '01-Visao-e-Escopo.pdf', 'Visão e escopo'],
  ['02', '02-requisitos.md', '02-Requisitos.pdf', 'Requisitos'],
  ['03', '03-casos-de-uso.md', '03-Casos-de-Uso.pdf', 'Casos de uso e histórias'],
  ['04', '04-modelagem.md', '04-Modelagem.pdf', 'Modelagem'],
  ['05', '05-arquitetura.md', '05-Arquitetura.pdf', 'Arquitetura e decisões'],
  ['06', '06-api-e-seguranca.md', '06-API-e-Seguranca.pdf', 'API e segurança'],
  ['07', '07-testes.md', '07-Testes.pdf', 'Testes'],
  ['08', '08-demo-limitacoes-e-roadmap.md', '08-Demo-Limitacoes-e-Roadmap.pdf', 'Demo, limitações e roadmap']
];

const marcador = /\]\((?:\.\/)?(\d{2})-[^)#\s]*\.md(?:#([^)]+))?\)/;

// Links entre documentos: no PDF único viram âncoras; nos PDFs separados viram uma referência por extenso.
function ajustarLinks(markdown, numeroAtual, unico) {
  let texto = markdown;

  // [texto](NN-arquivo.md#ancora) e [texto](NN-arquivo.md)
  texto = texto.replace(/\[([^\]]+)\]\(((?:\.\/)?\d{2}-[^)\s]*\.md(?:#[^)]+)?)\)/g, (_, rotulo, alvo) => {
    const [, numero, ancora] = alvo.match(marcador) ?? [];
    const mesmo = numero === numeroAtual;

    if (unico || mesmo) {
      return ancora ? `[${rotulo}](#${ancora})` : `[${rotulo}](#doc-${numero})`;
    }

    return `${rotulo} (documento ${Number(numero)})`;
  });

  // [texto](README.md#ancora): a legenda fica no índice
  texto = texto.replace(/\[([^\]]+)\]\(README\.md#([^)]+)\)/g, (_, rotulo, ancora) =>
    unico || numeroAtual === '00' ? `[${rotulo}](#${ancora})` : `${rotulo} (veja o índice, documento 0)`);

  // links para fora de docs/ (README da raiz e da API) viram texto
  texto = texto.replace(/\[([^\]]+)\]\(\.\.\/[^)]*\)/g, '$1');

  return texto;
}

const modelo = (titulo, dados, capa, cabecalho) => `<!doctype html>
<html lang="pt-BR">
<head>
<meta charset="utf-8">
<title>${titulo}</title>
<style>
  @page { size: A4; margin: 16mm 14mm; }
  * { box-sizing: border-box; }
  body { font-family: 'Segoe UI', Arial, sans-serif; font-size: 10.5pt; line-height: 1.5; color: #1d2b24; margin: 0; }
  a { color: #237463; text-decoration: none; }
  .cabecalho { font-size: 8.5pt; color: #76857e; letter-spacing: .06em; text-transform: uppercase; margin-bottom: 10px; }
  .capa { height: 255mm; display: flex; flex-direction: column; justify-content: center; page-break-after: always; }
  .capa .selo { font-size: 10pt; letter-spacing: .12em; text-transform: uppercase; color: #237463; font-weight: 700; }
  .capa h1 { font-size: 34pt; margin: 8px 0 6px; border: 0; padding: 0; color: #1d2b24; }
  .capa p { font-size: 13pt; color: #5b6b63; margin: 4px 0; }
  .capa .meta { margin-top: 36px; font-size: 10.5pt; color: #5b6b63; }
  section.doc { page-break-before: always; }
  section.doc:first-of-type { page-break-before: auto; }
  h1 { font-size: 22pt; color: #1d2b24; border-bottom: 3px solid #237463; padding-bottom: 6px; margin-top: 0; }
  h2 { font-size: 15pt; color: #237463; margin-top: 24px; page-break-after: avoid; }
  h3 { font-size: 12pt; color: #1d2b24; margin-top: 18px; page-break-after: avoid; }
  h4 { font-size: 11pt; page-break-after: avoid; }
  p, li { orphans: 3; widows: 3; }
  table { border-collapse: collapse; width: 100%; margin: 10px 0 14px; font-size: 9pt; page-break-inside: auto; }
  tr { page-break-inside: avoid; }
  th { background: #eaf3ef; color: #1d4d42; text-align: left; }
  th, td { border: 1px solid #d3ded8; padding: 5px 8px; vertical-align: top; }
  code { font-family: Consolas, 'Courier New', monospace; font-size: 9pt; background: #f1f5f3; padding: 1px 4px; border-radius: 3px; }
  pre { background: #f6f8f7; border: 1px solid #e0e8e4; border-radius: 6px; padding: 10px 12px; overflow: hidden; white-space: pre-wrap; page-break-inside: avoid; }
  pre code { background: none; padding: 0; }
  blockquote { margin: 12px 0; padding: 8px 14px; border-left: 4px solid #237463; background: #f4f7f0; color: #3f5249; }
  blockquote p { margin: 4px 0; }
  hr { border: 0; border-top: 1px solid #d3ded8; }
  .mermaid { text-align: center; margin: 14px 0; page-break-inside: avoid; }
  .mermaid svg { max-width: 100%; height: auto; }
</style>
</head>
<body>
  ${capa}
  ${cabecalho}
  <div id="conteudo"></div>

<script src="https://cdn.jsdelivr.net/npm/marked@12.0.2/marked.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js"></script>
<script>
const DOCS = ${dados};

// Mesma regra de âncoras do GitHub: minúsculas, sem pontuação, espaços viram hífen.
function slug(texto) {
  return texto.trim().toLowerCase().replace(/[^\\p{L}\\p{N}\\s-]/gu, '').replace(/\\s/g, '-');
}

(async () => {
  const raiz = document.getElementById('conteudo');

  for (const doc of DOCS) {
    const secao = document.createElement('section');
    secao.className = 'doc';
    secao.id = doc.id;
    secao.innerHTML = marked.parse(doc.md, { gfm: true });
    raiz.appendChild(secao);
  }

  document.querySelectorAll('section.doc h1, h2, h3, h4').forEach(h => {
    if (!h.id) h.id = slug(h.textContent);
  });

  document.querySelectorAll('pre code.language-mermaid').forEach(codigo => {
    const bloco = document.createElement('div');
    bloco.className = 'mermaid';
    bloco.textContent = codigo.textContent;
    codigo.parentElement.replaceWith(bloco);
  });

  mermaid.initialize({ startOnLoad: false, securityLevel: 'loose', theme: 'default', flowchart: { htmlLabels: true } });

  try {
    await mermaid.run({ querySelector: '.mermaid', suppressErrors: false });
  } catch (erro) {
    document.body.setAttribute('data-erro-mermaid', String(erro && erro.message || erro));
  }

  document.body.setAttribute('data-pronto', '1');
})();
</script>
</body>
</html>`;

function gerarPdf(nomeBase, titulo, lista, saidaPdf, { capa = '', cabecalho = '' } = {}) {
  const dados = JSON.stringify(lista).replace(/</g, '\\u003c');
  const arquivoHtml = join(pastaTemporaria, `${nomeBase}.html`);
  const perfil = join(pastaTemporaria, `perfil-${nomeBase}`);
  mkdirSync(perfil);
  writeFileSync(arquivoHtml, modelo(titulo, dados, capa, cabecalho), 'utf8');

  const urlHtml = pathToFileURL(arquivoHtml).href;
  const base = ['--headless=new', '--disable-gpu', `--user-data-dir=${perfil}`, '--virtual-time-budget=45000'];

  // 1) Valida: todo diagrama precisa virar SVG, sem erro de sintaxe.
  const dom = execFileSync(chrome, [...base, '--dump-dom', urlHtml], { maxBuffer: 256 * 1024 * 1024, stdio: ['ignore', 'pipe', 'ignore'] }).toString('utf8');
  const total = (dom.match(/class="mermaid"/g) ?? []).length;
  const desenhados = (dom.match(/<div class="mermaid"[^>]*data-processed="true"/g) ?? []).length;
  const erros = (dom.match(/Syntax error in text|mermaid version/g) ?? []).length;
  const erroGeral = dom.match(/data-erro-mermaid="([^"]*)"/)?.[1];

  if (!dom.includes('data-pronto="1"') || erros > 0 || desenhados !== total || erroGeral) {
    console.error(`FALHA em ${titulo}: diagramas ${desenhados}/${total}, erros ${erros}. ${erroGeral ?? ''}`);
    console.error(`HTML para inspeção: ${arquivoHtml}`);
    process.exit(1);
  }

  // 2) Gera o PDF.
  execFileSync(chrome, [...base, '--no-pdf-header-footer', `--print-to-pdf=${saidaPdf}`, urlHtml], { stdio: 'ignore' });
  const paginas = (readFileSync(saidaPdf).toString('latin1').match(/\/Type\s*\/Page[^s]/g) ?? []).length;
  console.log(`${titulo}: ${paginas} páginas, ${desenhados} diagramas -> ${saidaPdf}`);
}

mkdirSync(pastaPdf, { recursive: true });

for (const [numero, arquivo, pdf, titulo] of documentos) {
  const md = ajustarLinks(readFileSync(join(docs, arquivo), 'utf8'), numero, false);
  const cabecalho = `<div class="cabecalho">Ampara · Documentação do projeto · ${numero === '00' ? 'Índice' : `Documento ${Number(numero)} — ${titulo}`}</div>`;
  gerarPdf(pdf.replace('.pdf', ''), `Ampara — ${titulo}`, [{ id: `doc-${numero}`, md }], join(pastaPdf, pdf), { cabecalho });
}

if (gerarCompleto) {
  const lista = documentos.map(([numero, arquivo]) => ({
    id: `doc-${numero}`,
    md: ajustarLinks(readFileSync(join(docs, arquivo), 'utf8'), numero, true)
  }));

  const capa = `<div class="capa">
    <span class="selo">Documentação do projeto</span>
    <h1>Ampara</h1>
    <p>Sistema de gestão e adoção animal</p>
    <p>ONG Patas do Bem</p>
    <div class="meta">Versão 1.0 · 24/11/2026<br>.NET 10 · PostgreSQL · Angular 22</div>
  </div>`;

  gerarPdf('completo', 'Ampara — Documentação do projeto', lista, join(docs, 'Ampara-Documentacao.pdf'), { capa });
}
