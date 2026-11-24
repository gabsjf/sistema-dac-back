import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { PainelService } from '../../../core/painel.service';
import {
  AnimalAdmin,
  MESES_ABREVIADOS,
  Paginado,
  classeSituacao,
  formatarData,
  formatarIdade,
  rotuloEspecie,
  rotuloPorte,
  rotuloSexo,
  rotuloSituacao
} from '../../../core/rotulos';

// Altura útil das barras, igual a .bar-pair em admin.css.
const ALTURA_BARRAS = 150;

@Component({
  selector: 'app-visao-geral',
  imports: [RouterLink],
  templateUrl: './visao-geral.html',
  styleUrl: './visao-geral.css'
})
export class VisaoGeral implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly painelService = inject(PainelService);

  readonly painel = this.painelService.painel;
  readonly erroPainel = this.painelService.erro;

  readonly recentes = signal<AnimalAdmin[]>([]);
  readonly carregandoRecentes = signal(true);
  readonly erroRecentes = signal(false);

  readonly mesAtual = computed(() => this.painel()?.lastSixMonths.at(-1) ?? null);

  readonly grafico = computed(() => {
    const painel = this.painel();

    if (!painel) {
      return [];
    }

    const maximo = Math.max(
      1,
      ...painel.lastSixMonths.flatMap(mes => [mes.intakes, mes.adoptions])
    );

    return painel.lastSixMonths.map(mes => ({
      rotulo: MESES_ABREVIADOS[mes.month - 1],
      acolhimentos: mes.intakes,
      adocoes: mes.adoptions,
      alturaAcolhimentos: this.altura(mes.intakes, maximo),
      alturaAdocoes: this.altura(mes.adoptions, maximo)
    }));
  });

  readonly semDadosNoGrafico = computed(() =>
    this.grafico().every(mes => mes.acolhimentos === 0 && mes.adocoes === 0)
  );

  readonly rotuloEspecie = rotuloEspecie;
  readonly rotuloSexo = rotuloSexo;
  readonly rotuloPorte = rotuloPorte;
  readonly rotuloSituacao = rotuloSituacao;
  readonly classeSituacao = classeSituacao;
  readonly formatarIdade = formatarIdade;
  readonly formatarData = formatarData;

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.painelService.carregar();
    this.carregarRecentes();
  }

  private carregarRecentes(): void {
    this.carregandoRecentes.set(true);
    this.erroRecentes.set(false);

    this.http.get<Paginado<AnimalAdmin>>('/api/animals?page=1&pageSize=4').subscribe({
      next: (resposta) => {
        this.recentes.set(resposta.items ?? []);
        this.carregandoRecentes.set(false);
      },
      error: () => {
        this.erroRecentes.set(true);
        this.carregandoRecentes.set(false);
      }
    });
  }

  private altura(valor: number, maximo: number): number {
    if (valor === 0) {
      return 0;
    }

    // Mantém uma barra visível mesmo quando o valor é pequeno perto do maior.
    return Math.max(6, Math.round((valor / maximo) * ALTURA_BARRAS));
  }
}
