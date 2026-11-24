import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface PontoMensal {
  year: number;
  month: number;
  intakes: number;
  adoptions: number;
}

export interface Painel {
  totalAnimals: number;
  available: number;
  inTreatment: number;
  adopted: number;
  pendingRequests: number;
  lastSixMonths: PontoMensal[];
}

// Indicadores de GET /api/dashboard, compartilhados entre o menu (selo de pendências) e as telas da equipe.
@Injectable({ providedIn: 'root' })
export class PainelService {
  private readonly http = inject(HttpClient);

  readonly painel = signal<Painel | null>(null);
  readonly erro = signal(false);

  carregar(): void {
    this.http.get<Painel>('/api/dashboard').subscribe({
      next: (painel) => {
        this.painel.set(painel);
        this.erro.set(false);
      },
      error: () => this.erro.set(true)
    });
  }
}
