import { HttpErrorResponse } from '@angular/common/http';

export interface AnimalAdmin {
  id: string;
  name: string;
  species: string;
  sex: string;
  size: string;
  approximateAgeMonths: number;
  intakeDate: string;
  description: string | null;
  isVaccinated: boolean;
  isNeutered: boolean;
  isDewormed: boolean;
  photoUrl: string | null;
  status: string;
}

export interface Paginado<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

const ESPECIES: Record<string, string> = {
  Canine: 'Canina',
  Feline: 'Felina',
  Other: 'Outra'
};

const SEXOS: Record<string, string> = {
  Female: 'Fêmea',
  Male: 'Macho'
};

const PORTES: Record<string, string> = {
  Small: 'Pequeno',
  Medium: 'Médio',
  Large: 'Grande'
};

const SITUACOES: Record<string, string> = {
  Available: 'Disponível',
  InTreatment: 'Em tratamento',
  Adopted: 'Adotado'
};

// Classes de .status-pill em admin.css.
const CLASSES_SITUACAO: Record<string, string> = {
  Available: 'disponivel',
  InTreatment: 'tratamento',
  Adopted: 'adaptacao'
};

export const MESES_ABREVIADOS = [
  'Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
  'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'
];

export const rotuloEspecie = (valor: string) => ESPECIES[valor] ?? valor;
export const rotuloSexo = (valor: string) => SEXOS[valor] ?? valor;
export const rotuloPorte = (valor: string) => PORTES[valor] ?? valor;
export const rotuloSituacao = (valor: string) => SITUACOES[valor] ?? valor;
export const classeSituacao = (valor: string) => CLASSES_SITUACAO[valor] ?? '';

export function formatarIdade(meses: number): string {
  if (meses < 12) {
    return meses === 1 ? '1 mês' : `${meses} meses`;
  }

  const anos = Math.floor(meses / 12);
  const resto = meses % 12;
  const textoAnos = anos === 1 ? '1 ano' : `${anos} anos`;

  if (resto === 0) {
    return textoAnos;
  }

  return `${textoAnos} e ${resto === 1 ? '1 mês' : `${resto} meses`}`;
}

// A API devolve datas sem horário ("2026-11-16"); montar a data local evita o deslocamento de fuso.
export function formatarData(iso: string): string {
  const [ano, mes, dia] = iso.slice(0, 10).split('-').map(Number);
  const data = new Date(ano, mes - 1, dia);
  const mesAbreviado = data
    .toLocaleDateString('pt-BR', { month: 'short' })
    .replace('.', '');

  return `${String(dia).padStart(2, '0')} ${mesAbreviado}, ${ano}`;
}

// Erros da API seguem o ProblemDetails: "detail" pode ser mostrado ao usuário.
export function mensagemErro(erro: HttpErrorResponse, padrao: string): string {
  if (erro.status === 0) {
    return 'Não foi possível falar com a API. Confira se ela está rodando.';
  }

  const corpo = erro.error;

  if (typeof corpo?.detail === 'string' && corpo.detail) {
    return corpo.detail;
  }

  if (corpo?.errors && typeof corpo.errors === 'object') {
    const primeiro = Object.values(corpo.errors as Record<string, string[]>)[0];

    if (Array.isArray(primeiro) && primeiro[0]) {
      return primeiro[0];
    }
  }

  return padrao;
}
