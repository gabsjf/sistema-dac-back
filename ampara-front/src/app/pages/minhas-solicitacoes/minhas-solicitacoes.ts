
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

interface Animal {
  id: string;
  name: string;
  species: string;
  sex: string;
  size: string;
  approximateAgeMonths: number;
  photoUrl: string | null;
}

interface Solicitacao {
  id: string;
  status: string;
  createdAt: string;
  decidedAt: string | null;
  rejectionReason: string | null;
  animal: Animal;
}

@Component({
  selector: 'app-minhas-solicitacoes',
  imports: [RouterLink],
  templateUrl: './minhas-solicitacoes.html',
  styleUrl: './minhas-solicitacoes.css'
})
export class MinhasSolicitacoes implements OnInit {

  private readonly http = inject(HttpClient);

  readonly solicitacoes = signal<Solicitacao[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal('');

  ngOnInit(): void {
    this.carregarSolicitacoes();
  }

  carregarSolicitacoes(): void {
    this.carregando.set(true);
    this.erro.set('');

    this.http.get<Solicitacao[]>('/api/adoptions/mine').subscribe({
      next: (dados) => {
        this.solicitacoes.set(dados);
        this.carregando.set(false);
      },
      error: (erro) => {
        console.error('Erro ao carregar solicitações:', erro);
        this.erro.set('Não foi possível carregar suas solicitações.');
        this.carregando.set(false);
      }
    });
  }

  statusTexto(status: string): string {
    const statusMap: Record<string, string> = {
      Pending: 'Em análise',
      Approved: 'Aprovada',
      Rejected: 'Rejeitada'
    };

    return statusMap[status] ?? status;
  }

  imagemAnimal(animal: Animal): string {
    if (animal.photoUrl) {
      return animal.photoUrl;
    }

    const imagens: Record<string, string> = {
      luna: '/assets/images/Luna-card.svg',
      chico: '/assets/images/Chico-card.svg',
      mel: '/assets/images/Nina-card.svg'
    };

    return imagens[animal.name.toLowerCase()] ??
      '/assets/images/Nina-card.svg';
  }

  dataFormatada(data: string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }
}
