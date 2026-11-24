
import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { PainelService } from '../../../core/painel.service';

interface Adocao {
  id: string;
  status: string;
  createdAt: string;
  decidedAt: string | null;
  rejectionReason: string | null;

  animal: {
    id: string;
    name: string;
    species: string;
    sex: string;
    size: string;
    approximateAgeMonths: number;
    photoUrl: string | null;
  };

  applicant: {
    id: string;
    fullName: string;
    email: string;
    phone: string;
    housing: string;
    routine: string;
    message: string | null;
  };
}

interface ResultadoPaginado {
  items: Adocao[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages?: number;
}

@Component({
  selector: 'app-adocoes',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './adocoes.html',
  styleUrl: './adocoes.css'
})
export class Adocoes implements OnInit {

  private readonly http = inject(HttpClient);
  private readonly painelService = inject(PainelService);

  readonly solicitacoes = signal<Adocao[]>([]);
  readonly carregando = signal(false);
  readonly erro = signal('');
  readonly total = signal(0);

  // Solicitação selecionada
  readonly adocaoSelecionada = signal<Adocao | null>(null);

  // Indicadores
  readonly pendentes = computed(() =>
    this.solicitacoes().filter(
      adocao => adocao.status === 'Pending'
    ).length
  );

  readonly aprovadas = computed(() =>
    this.solicitacoes().filter(
      adocao => adocao.status === 'Approved'
    ).length
  );

  readonly rejeitadas = computed(() =>
    this.solicitacoes().filter(
      adocao => adocao.status === 'Rejected'
    ).length
  );

  // Solicitações recentes
  readonly recentes = computed(() =>
    this.solicitacoes()
      .filter(adocao => adocao.status === 'Pending')
      .sort(
        (a, b) =>
          new Date(b.createdAt).getTime() -
          new Date(a.createdAt).getTime()
      )
  );

  // Solicitações em análise
  readonly emAnalise = computed(() =>
    this.solicitacoes().filter(
      adocao => adocao.status === 'Pending'
    )
  );

  // Adoções aprovadas
  readonly concluidas = computed(() =>
    this.solicitacoes().filter(
      adocao => adocao.status === 'Approved'
    )
  );

  ngOnInit(): void {
    this.carregarSolicitacoes();
  }

  // Buscar solicitações na API
  carregarSolicitacoes(): void {
    this.carregando.set(true);
    this.erro.set('');

    this.http.get<ResultadoPaginado>(
      '/api/adoptions?page=1&pageSize=100'
    ).subscribe({
      next: (resposta) => {
        this.solicitacoes.set(resposta.items ?? []);
        this.total.set(resposta.totalCount ?? 0);
        this.carregando.set(false);

        // Mantém o selo de pendências do menu em dia.
        this.painelService.carregar();

        // Atualiza também a solicitação aberta
        const selecionada = this.adocaoSelecionada();

        if (selecionada) {
          const atualizada = resposta.items?.find(
            item => item.id === selecionada.id
          );

          this.adocaoSelecionada.set(atualizada ?? null);
        }
      },

      error: (erro: HttpErrorResponse) => {
        this.carregando.set(false);

        if (erro.status === 401) {
          this.erro.set(
            'Sua sessão expirou. Entre novamente no sistema.'
          );
        } else if (erro.status === 403) {
          this.erro.set(
            'Você não tem autorização para acessar esta área.'
          );
        } else {
          this.erro.set(
            'Não foi possível carregar as solicitações.'
          );
        }
      }
    });
  }

  // Abrir detalhes
  abrirDetalhes(adocao: Adocao): void {
    this.adocaoSelecionada.set(adocao);
  }

  // Fechar detalhes
  fecharDetalhes(): void {
    this.adocaoSelecionada.set(null);
  }

  // Aprovar adoção
  aprovar(adocao: Adocao): void {
    const confirmar = window.confirm(
      `Deseja aprovar a adoção de ${adocao.animal.name} para ${adocao.applicant.fullName}?`
    );

    if (!confirmar) {
      return;
    }

    this.erro.set('');

    this.http.post(
      `/api/adoptions/${adocao.id}/approve`,
      {}
    ).subscribe({
      next: () => {
        this.fecharDetalhes();
        this.carregarSolicitacoes();
      },

      error: () => {
        this.erro.set(
          'Não foi possível aprovar a solicitação.'
        );
      }
    });
  }

  // Rejeitar adoção
  rejeitar(adocao: Adocao): void {
    const motivo = window.prompt(
      'Informe o motivo da rejeição:'
    );

    if (!motivo || !motivo.trim()) {
      return;
    }

    this.erro.set('');

    this.http.post(
      `/api/adoptions/${adocao.id}/reject`,
      {
        reason: motivo.trim()
      }
    ).subscribe({
      next: () => {
        this.fecharDetalhes();
        this.carregarSolicitacoes();
      },

      error: () => {
        this.erro.set(
          'Não foi possível rejeitar a solicitação.'
        );
      }
    });
  }

  // Desfazer aprovação de adoção
  desfazerAprovacao(adocao: Adocao): void {
    if (adocao.status !== 'Approved') {
      return;
    }

    const confirmar = window.confirm(
      `Deseja realmente desfazer a aprovação da adoção de ${adocao.animal.name} para ${adocao.applicant.fullName}?\n\nO animal voltará a ficar disponível e a solicitação retornará para análise.`
    );

    if (!confirmar) {
      return;
    }

    this.erro.set('');

    this.http.post(
      `/api/adoptions/${adocao.id}/undo-approval`,
      {}
    ).subscribe({
      next: () => {
        this.fecharDetalhes();
        this.carregarSolicitacoes();
      },

      error: (erro: HttpErrorResponse) => {
        if (erro.status === 401) {
          this.erro.set(
            'Sua sessão expirou. Entre novamente.'
          );
        } else if (erro.status === 403) {
          this.erro.set(
            'Você não tem autorização para realizar esta ação.'
          );
        } else {
          this.erro.set(
            'Não foi possível desfazer a aprovação.'
          );
        }
      }
    });
  }

  // Traduzir status
  nomeStatus(status: string): string {
    switch (status) {
      case 'Pending':
        return 'Em análise';

      case 'Approved':
        return 'Aprovada';

      case 'Rejected':
        return 'Rejeitada';

      default:
        return status;
    }
  }

  // Formatar idade do animal
  idadeAnimal(meses: number): string {
    if (meses < 12) {
      return `${meses} meses`;
    }

    const anos = Math.floor(meses / 12);
    const resto = meses % 12;

    if (resto === 0) {
      return `${anos} ${anos === 1 ? 'ano' : 'anos'}`;
    }

    return `${anos} ${anos === 1 ? 'ano' : 'anos'} e ${resto} meses`;
  }
}
