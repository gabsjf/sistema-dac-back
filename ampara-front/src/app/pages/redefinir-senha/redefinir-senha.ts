import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RecuperarSenhaService } from '../../core/recuperar-senha.service';

@Component({
  selector: 'app-redefinir-senha',
  imports: [FormsModule, RouterLink],
  templateUrl: './redefinir-senha.html',
  styleUrl: '../login/login.css'
})
export class RedefinirSenha {
  private readonly recuperarSenha = inject(RecuperarSenhaService);
  private readonly rota = inject(ActivatedRoute);

  readonly token = this.rota.snapshot.queryParamMap.get('token') ?? '';

  novaSenha = '';
  confirmacao = '';

  readonly salvando = signal(false);
  readonly concluido = signal(false);
  readonly erro = signal('');

  salvar(): void {
    if (this.salvando()) {
      return;
    }

    this.erro.set('');

    if (this.novaSenha.length < 8) {
      this.erro.set('A senha deve ter pelo menos 8 caracteres.');
      return;
    }

    if (this.novaSenha !== this.confirmacao) {
      this.erro.set('As senhas não conferem.');
      return;
    }

    this.salvando.set(true);

    this.recuperarSenha.redefinir(this.token, this.novaSenha).subscribe({
      next: () => {
        this.salvando.set(false);
        this.concluido.set(true);
      },
      error: () => {
        this.salvando.set(false);
        this.erro.set('Não foi possível redefinir a senha. Tente novamente.');
      }
    });
  }
}
