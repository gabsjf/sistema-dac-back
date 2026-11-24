import { Component, HostListener, OnInit, computed, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PainelService } from '../../../core/painel.service';

interface UsuarioSessao {
  id: string;
  fullName: string;
  email: string;
  role: string;
}

type MenuAberto = 'notificacoes' | 'perfil' | null;

@Component({
  selector: 'app-admin-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './admin-shell.html',
  styleUrl: './admin-shell.css'
})
export class AdminShell implements OnInit {
  private readonly router = inject(Router);
  private readonly painelService = inject(PainelService);

  readonly usuario = this.lerUsuario();
  readonly ehSuperAdmin = this.usuario?.role === 'SuperAdmin';
  readonly papelRotulo = this.ehSuperAdmin ? 'Super Admin' : 'Admin';
  readonly iniciais = this.calcularIniciais(this.usuario?.fullName ?? '');

  readonly pendentes = computed(
    () => this.painelService.painel()?.pendingRequests ?? 0
  );

  readonly menuAberto = signal<MenuAberto>(null);

  busca = '';

  ngOnInit(): void {
    this.painelService.carregar();
  }

  alternarMenu(menu: Exclude<MenuAberto, null>): void {
    this.menuAberto.update(atual => (atual === menu ? null : menu));
  }

  @HostListener('document:click', ['$event'])
  fecharMenus(evento: MouseEvent): void {
    const alvo = evento.target as HTMLElement | null;

    if (!alvo?.closest('.popover-anchor')) {
      this.menuAberto.set(null);
    }
  }

  @HostListener('document:keydown.escape')
  fecharMenusComEsc(): void {
    this.menuAberto.set(null);
  }

  buscar(): void {
    const termo = this.busca.trim();

    this.router.navigate(['/admin/animais'], {
      queryParams: termo ? { busca: termo } : {}
    });
  }

  sair(): void {
    sessionStorage.removeItem('ampara_token');
    sessionStorage.removeItem('ampara_usuario');
    sessionStorage.removeItem('ampara_expiracao');

    this.router.navigateByUrl('/');
  }

  private lerUsuario(): UsuarioSessao | null {
    try {
      return JSON.parse(sessionStorage.getItem('ampara_usuario') ?? '') as UsuarioSessao;
    } catch {
      return null;
    }
  }

  private calcularIniciais(nome: string): string {
    const partes = nome.trim().split(/\s+/).filter(Boolean);

    if (partes.length === 0) {
      return '?';
    }

    const primeira = partes[0][0];
    const ultima = partes.length > 1 ? partes[partes.length - 1][0] : '';

    return (primeira + ultima).toUpperCase();
  }
}
