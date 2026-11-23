
import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = sessionStorage.getItem('ampara_token');

  // Não envia token para serviços externos.
  if (!req.url.startsWith('/api/') || !token) {
    return next(req);
  }

  const requisicaoAutenticada = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(requisicaoAutenticada);
};
