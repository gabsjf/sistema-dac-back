import { Component, HostListener, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PainelService } from '../../../core/painel.service';
import {
  AnimalAdmin,
  Paginado,
  classeSituacao,
  formatarData,
  formatarIdade,
  mensagemErro,
  rotuloEspecie,
  rotuloPorte,
  rotuloSexo,
  rotuloSituacao
} from '../../../core/rotulos';

interface FormularioAnimal {
  name: string;
  species: string;
  sex: string;
  size: string;
  ageMonths: number | null;
  intakeDate: string;
  description: string;
  isVaccinated: boolean;
  isNeutered: boolean;
  isDewormed: boolean;
  status: string;
}

const TAMANHO_PAGINA = 10;
const TAMANHO_MAXIMO_FOTO = 5 * 1024 * 1024;
const IDADE_MAXIMA_MESES = 360;

function hoje(): string {
  const agora = new Date();
  const mes = String(agora.getMonth() + 1).padStart(2, '0');
  const dia = String(agora.getDate()).padStart(2, '0');

  return `${agora.getFullYear()}-${mes}-${dia}`;
}

function formularioVazio(): FormularioAnimal {
  return {
    name: '',
    species: 'Canine',
    sex: 'Female',
    size: 'Medium',
    ageMonths: null,
    intakeDate: hoje(),
    description: '',
    isVaccinated: false,
    isNeutered: false,
    isDewormed: false,
    status: 'Available'
  };
}

@Component({
  selector: 'app-animais',
  imports: [FormsModule, RouterLink],
  templateUrl: './animais.html',
  styleUrl: './animais.css'
})
export class Animais implements OnInit, OnDestroy {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly painelService = inject(PainelService);

  readonly painel = this.painelService.painel;

  // Listagem
  readonly animais = signal<AnimalAdmin[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal('');
  readonly aviso = signal('');
  readonly pagina = signal(1);
  readonly totalPaginas = signal(1);
  readonly total = signal(0);

  // Filtros (os campos do formulário só valem depois de "Aplicar filtros")
  busca = '';
  especie = '';
  situacao = '';
  private aplicado = { busca: '', especie: '', situacao: '' };
  private iniciou = false;
  private temporizadorAviso: ReturnType<typeof setTimeout> | null = null;

  // Cadastro e edição
  readonly modalAberto = signal(false);
  readonly editando = signal<AnimalAdmin | null>(null);
  readonly salvando = signal(false);
  readonly erroForm = signal('');
  readonly fotoPrevia = signal<string | null>(null);
  form: FormularioAnimal = formularioVazio();
  arquivoFoto: File | null = null;

  readonly idadeMaxima = IDADE_MAXIMA_MESES;

  readonly rotuloEspecie = rotuloEspecie;
  readonly rotuloSexo = rotuloSexo;
  readonly rotuloPorte = rotuloPorte;
  readonly rotuloSituacao = rotuloSituacao;
  readonly classeSituacao = classeSituacao;
  readonly formatarIdade = formatarIdade;
  readonly formatarData = formatarData;

  ngOnInit(): void {
    this.painelService.carregar();

    // Emite já na primeira leitura e a cada mudança (busca do topo, "Cadastrar animal", "Editar").
    this.route.queryParamMap.subscribe((params) => {
      const buscaDaUrl = params.get('busca') ?? '';

      if (!this.iniciou || buscaDaUrl !== this.aplicado.busca) {
        this.iniciou = true;
        this.busca = buscaDaUrl;
        this.aplicado = { ...this.aplicado, busca: buscaDaUrl };
        this.pagina.set(1);
        this.carregar();
      }

      const editar = params.get('editar');

      if (params.has('novo')) {
        this.abrirNovo();
        this.limparParametro('novo');
      } else if (editar) {
        this.abrirEdicaoPorId(editar);
        this.limparParametro('editar');
      }
    });
  }

  ngOnDestroy(): void {
    this.liberarPrevia();

    if (this.temporizadorAviso) {
      clearTimeout(this.temporizadorAviso);
    }
  }

  @HostListener('document:keydown.escape')
  fecharComEsc(): void {
    if (this.modalAberto() && !this.salvando()) {
      this.fecharModal();
    }
  }

  carregar(): void {
    this.carregando.set(true);
    this.erro.set('');

    let params = new HttpParams()
      .set('page', this.pagina())
      .set('pageSize', TAMANHO_PAGINA);

    if (this.aplicado.busca) {
      params = params.set('search', this.aplicado.busca);
    }

    if (this.aplicado.especie) {
      params = params.set('species', this.aplicado.especie);
    }

    if (this.aplicado.situacao) {
      params = params.set('status', this.aplicado.situacao);
    }

    this.http.get<Paginado<AnimalAdmin>>('/api/animals', { params }).subscribe({
      next: (resposta) => {
        this.animais.set(resposta.items ?? []);
        this.total.set(resposta.totalCount ?? 0);
        this.totalPaginas.set(Math.max(1, resposta.totalPages ?? 1));
        this.carregando.set(false);
      },
      error: (erro) => {
        this.erro.set(mensagemErro(erro, 'Não foi possível carregar os animais.'));
        this.carregando.set(false);
      }
    });
  }

  aplicarFiltros(): void {
    this.aplicado = {
      busca: this.busca.trim(),
      especie: this.especie,
      situacao: this.situacao
    };

    this.pagina.set(1);
    this.carregar();

    // Mantém a busca na URL; como já bate com o filtro aplicado, não recarrega duas vezes.
    this.router.navigate([], {
      queryParams: { busca: this.aplicado.busca || null },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
  }

  limparFiltros(): void {
    this.busca = '';
    this.especie = '';
    this.situacao = '';
    this.aplicarFiltros();
  }

  irParaPagina(numero: number): void {
    if (numero < 1 || numero > this.totalPaginas() || numero === this.pagina()) {
      return;
    }

    this.pagina.set(numero);
    this.carregar();
  }

  // Cadastro e edição

  abrirNovo(): void {
    this.editando.set(null);
    this.form = formularioVazio();
    this.prepararModal();
  }

  abrirEdicao(animal: AnimalAdmin): void {
    this.editando.set(animal);
    this.form = {
      name: animal.name,
      species: animal.species,
      sex: animal.sex,
      size: animal.size,
      ageMonths: animal.approximateAgeMonths,
      intakeDate: animal.intakeDate.slice(0, 10),
      description: animal.description ?? '',
      isVaccinated: animal.isVaccinated,
      isNeutered: animal.isNeutered,
      isDewormed: animal.isDewormed,
      status: animal.status
    };
    this.prepararModal();
  }

  private abrirEdicaoPorId(id: string): void {
    const naLista = this.animais().find(animal => animal.id === id);

    if (naLista) {
      this.abrirEdicao(naLista);
      return;
    }

    this.http.get<AnimalAdmin>(`/api/animals/${id}`).subscribe({
      next: (animal) => this.abrirEdicao(animal),
      error: (erro) =>
        this.erro.set(mensagemErro(erro, 'Não foi possível abrir este animal.'))
    });
  }

  private prepararModal(): void {
    this.liberarPrevia();
    this.arquivoFoto = null;
    this.erroForm.set('');
    this.modalAberto.set(true);
  }

  fecharModal(): void {
    if (this.salvando()) {
      return;
    }

    this.liberarPrevia();
    this.arquivoFoto = null;
    this.modalAberto.set(false);
  }

  aoEscolherFoto(evento: Event): void {
    const campo = evento.target as HTMLInputElement;
    const arquivo = campo.files?.[0] ?? null;

    this.erroForm.set('');
    this.liberarPrevia();
    this.arquivoFoto = null;

    if (!arquivo) {
      return;
    }

    if (arquivo.type !== 'image/jpeg' && arquivo.type !== 'image/png') {
      this.erroForm.set('A foto precisa ser JPG ou PNG.');
      campo.value = '';
      return;
    }

    if (arquivo.size > TAMANHO_MAXIMO_FOTO) {
      this.erroForm.set('Envie uma foto de até 5 MB.');
      campo.value = '';
      return;
    }

    this.arquivoFoto = arquivo;
    this.fotoPrevia.set(URL.createObjectURL(arquivo));
  }

  fotoExibida(): string | null {
    return this.fotoPrevia() ?? this.editando()?.photoUrl ?? null;
  }

  salvar(): void {
    if (this.salvando()) {
      return;
    }

    const nome = this.form.name.trim();
    const idade = this.form.ageMonths;

    if (!nome) {
      this.erroForm.set('Informe o nome do animal.');
      return;
    }

    if (idade === null || !Number.isInteger(idade) || idade < 0 || idade > IDADE_MAXIMA_MESES) {
      this.erroForm.set(`Informe a idade aproximada em meses (de 0 a ${IDADE_MAXIMA_MESES}).`);
      return;
    }

    if (!this.form.intakeDate) {
      this.erroForm.set('Informe a data de acolhimento.');
      return;
    }

    const corpo = {
      name: nome,
      species: this.form.species,
      sex: this.form.sex,
      size: this.form.size,
      approximateAgeMonths: idade,
      intakeDate: this.form.intakeDate,
      description: this.form.description.trim() || null,
      isVaccinated: this.form.isVaccinated,
      isNeutered: this.form.isNeutered,
      isDewormed: this.form.isDewormed,
      status: this.form.status
    };

    const existente = this.editando();

    this.erroForm.set('');
    this.salvando.set(true);

    const requisicao = existente
      ? this.http.put<AnimalAdmin>(`/api/animals/${existente.id}`, corpo)
      : this.http.post<AnimalAdmin>('/api/animals', corpo);

    requisicao.subscribe({
      next: (animal) => {
        if (this.arquivoFoto) {
          this.enviarFoto(animal, !existente);
        } else {
          this.concluir(!existente);
        }
      },
      error: (erro) => {
        this.salvando.set(false);
        this.erroForm.set(mensagemErro(erro, 'Não foi possível salvar o animal.'));
      }
    });
  }

  private enviarFoto(animal: AnimalAdmin, novo: boolean): void {
    const dados = new FormData();
    dados.append('file', this.arquivoFoto as File);

    this.http.post<AnimalAdmin>(`/api/animals/${animal.id}/photo`, dados).subscribe({
      next: () => this.concluir(novo),
      error: (erro) => {
        // O animal já foi salvo: passa a editá-lo para um novo envio não criar outro cadastro.
        this.salvando.set(false);
        this.editando.set(animal);
        this.arquivoFoto = null;
        this.carregar();
        this.painelService.carregar();
        this.erroForm.set(
          'O animal foi salvo, mas a foto não foi enviada. ' +
          mensagemErro(erro, 'Tente enviar a foto de novo.')
        );
      }
    });
  }

  private concluir(novo: boolean): void {
    this.salvando.set(false);
    this.liberarPrevia();
    this.arquivoFoto = null;
    this.modalAberto.set(false);

    if (novo) {
      this.pagina.set(1);
    }

    this.carregar();
    this.painelService.carregar();
    this.mostrarAviso(novo ? 'Animal cadastrado com sucesso.' : 'Animal atualizado com sucesso.');
  }

  private mostrarAviso(texto: string): void {
    this.aviso.set(texto);

    if (this.temporizadorAviso) {
      clearTimeout(this.temporizadorAviso);
    }

    this.temporizadorAviso = setTimeout(() => this.aviso.set(''), 5000);
  }

  private liberarPrevia(): void {
    const previa = this.fotoPrevia();

    if (previa) {
      URL.revokeObjectURL(previa);
      this.fotoPrevia.set(null);
    }
  }

  private limparParametro(nome: string): void {
    this.router.navigate([], {
      queryParams: { [nome]: null },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
  }
}
