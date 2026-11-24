import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';

// MOCK: ainda não existe API de recuperação de senha.
// Quando existir, troque o corpo destes métodos por chamadas HttpClient.
@Injectable({ providedIn: 'root' })
export class RecuperarSenhaService {
  solicitarLink(_email: string): Observable<void> {
    return of(undefined).pipe(delay(800));
  }

  redefinir(_token: string, _novaSenha: string): Observable<void> {
    return of(undefined).pipe(delay(800));
  }
}
