import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RecuperarSenhaService } from '../../core/recuperar-senha.service';

@Component({
  selector: 'app-esqueci-senha',
  imports: [FormsModule, RouterLink],
  templateUrl: './esqueci-senha.html',
  styleUrl: '../login/login.css'
})
export class EsqueciSenha {
  private readonly recuperarSenha = inject(RecuperarSenhaService);

  email = '';

  readonly enviando = signal(false);
  readonly enviado = signal(false);
  readonly erro = signal('');

  enviar(): void {
    if (this.enviando()) {
      return;
    }

    this.erro.set('');

    if (!this.email.trim()) {
      this.erro.set('Informe o e-mail da sua conta.');
      return;
    }

    this.enviando.set(true);

    this.recuperarSenha.solicitarLink(this.email.trim()).subscribe({
      next: () => {
        this.enviando.set(false);
        this.enviado.set(true);
      },
      error: () => {
        this.enviando.set(false);
        this.erro.set('Não foi possível enviar o link. Tente novamente.');
      }
    });
  }
}
