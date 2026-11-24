import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Paginado, mensagemErro } from '../../../core/rotulos';

interface Pessoa {
  id: string;
  fullName: string;
  email: string;
  phone: string | null;
  role: string;
}

const TAMANHO_PAGINA = 10;

@Component({
  selector: 'app-equipe',
  imports: [FormsModule],
  templateUrl: './equipe.html',
  styleUrl: './equipe.css'
})
export class Equipe implements OnInit {
  private readonly http = inject(HttpClient);

  readonly pessoas = signal<Pessoa[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal('');
  readonly aviso = signal('');
  readonly salvandoId = signal<string | null>(null);
  readonly pagina = signal(1);
  readonly totalPaginas = signal(1);
  readonly total = signal(0);

  busca = '';
  private buscaAplicada = '';
  private temporizadorAviso: ReturnType<typeof setTimeout> | null = null;

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.carregando.set(true);
    this.erro.set('');

    let params = new HttpParams()
      .set('page', this.pagina())
      .set('pageSize', TAMANHO_PAGINA);

    if (this.buscaAplicada) {
      params = params.set('search', this.buscaAplicada);
    }

    this.http.get<Paginado<Pessoa>>('/api/users', { params }).subscribe({
      next: (resposta) => {
        this.pessoas.set(resposta.items ?? []);
        this.total.set(resposta.totalCount ?? 0);
        this.totalPaginas.set(Math.max(1, resposta.totalPages ?? 1));
        this.carregando.set(false);
      },
      error: (erro) => {
        this.erro.set(mensagemErro(erro, 'Não foi possível carregar a equipe.'));
        this.carregando.set(false);
      }
    });
  }

  buscar(): void {
    this.buscaAplicada = this.busca.trim();
    this.pagina.set(1);
    this.carregar();
  }

  irParaPagina(numero: number): void {
    if (numero < 1 || numero > this.totalPaginas() || numero === this.pagina()) {
      return;
    }

    this.pagina.set(numero);
    this.carregar();
  }

  mudarPapel(pessoa: Pessoa, novoPapel: string): void {
    if (novoPapel === pessoa.role || this.salvandoId()) {
      return;
    }

    this.salvandoId.set(pessoa.id);
    this.erro.set('');

    this.http.put<Pessoa>(`/api/users/${pessoa.id}/role`, { role: novoPapel }).subscribe({
      next: (atualizada) => {
        this.pessoas.update(lista =>
          lista.map(item => (item.id === atualizada.id ? atualizada : item))
        );
        this.salvandoId.set(null);
        this.mostrarAviso(
          `${atualizada.fullName} agora é ${atualizada.role === 'Admin' ? 'Admin' : 'adotante'}.`
        );
      },
      error: (erro) => {
        this.salvandoId.set(null);
        this.erro.set(mensagemErro(erro, 'Não foi possível alterar o papel.'));
        // Volta o seletor ao valor que está salvo.
        this.carregar();
      }
    });
  }

  private mostrarAviso(texto: string): void {
    this.aviso.set(texto);

    if (this.temporizadorAviso) {
      clearTimeout(this.temporizadorAviso);
    }

    this.temporizadorAviso = setTimeout(() => this.aviso.set(''), 5000);
  }
}
