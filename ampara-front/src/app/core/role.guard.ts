import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

function papelAtual(): string | null {
  if (!sessionStorage.getItem('ampara_token')) {
    return null;
  }

  try {
    const usuario = JSON.parse(sessionStorage.getItem('ampara_usuario') ?? '');
    return typeof usuario?.role === 'string' ? usuario.role : null;
  } catch {
    // Dados de sessão inválidos.
    return null;
  }
}

function ehEquipe(papel: string): boolean {
  return papel === 'Admin' || papel === 'SuperAdmin';
}

// Telas do adotante: a equipe não pede adoção, então volta para o seu painel.
export const adotanteGuard: CanActivateFn = () => {
  const router = inject(Router);
  const papel = papelAtual();

  if (!papel) {
    return router.parseUrl('/entrar');
  }

  return ehEquipe(papel) ? router.parseUrl('/admin/visao-geral') : true;
};

// Telas da equipe (Admin e SuperAdmin).
export const equipeGuard: CanActivateFn = () => {
  const router = inject(Router);
  const papel = papelAtual();

  if (!papel) {
    return router.parseUrl('/entrar');
  }

  return ehEquipe(papel) ? true : router.parseUrl('/');
};

// Gestão da equipe: só o SuperAdmin promove ou rebaixa pessoas.
export const superAdminGuard: CanActivateFn = () => {
  const router = inject(Router);

  return papelAtual() === 'SuperAdmin' ? true : router.parseUrl('/admin/visao-geral');
};
