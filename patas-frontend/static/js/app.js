document.addEventListener('DOMContentLoaded', () => {
    const openTriggers = document.querySelectorAll('[data-open-modal]');
    const closeTriggers = document.querySelectorAll('[data-close-modal]');
    const backdrops = document.querySelectorAll('.modal-backdrop');

    function openModal(modalId, triggerEl) {
        const modal = document.getElementById(modalId);
        if (!modal) return;

        if (modalId === 'modal-adocao' && triggerEl) {
            const animalId = triggerEl.getAttribute('data-animal-id') || '';
            const animalNome = triggerEl.getAttribute('data-animal-nome') || '';
            const idInput = document.getElementById('adocao-animal-id');
            const nomeInput = document.getElementById('adocao-animal-nome');
            const tituloEl = document.getElementById('modal-adocao-titulo');

            if (idInput) idInput.value = animalId;
            if (nomeInput) nomeInput.value = animalNome;
            if (tituloEl && animalNome) {
                tituloEl.textContent = `Solicitar adoção · ${animalNome}`;
            }
        }

        modal.classList.add('is-open');
        document.body.style.overflow = 'hidden';
    }

    function closeModal(modal) {
        if (!modal) return;
        modal.classList.remove('is-open');
        document.body.style.overflow = '';
    }

    openTriggers.forEach(btn => {
        btn.addEventListener('click', () => {
            const target = btn.getAttribute('data-open-modal');
            openModal(target, btn);
        });
    });

    closeTriggers.forEach(btn => {
        btn.addEventListener('click', () => {
            const modal = btn.closest('.modal-backdrop');
            closeModal(modal);
        });
    });

    backdrops.forEach(backdrop => {
        backdrop.addEventListener('click', (event) => {
            if (event.target === backdrop) {
                closeModal(backdrop);
            }
        });
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') {
            document.querySelectorAll('.modal-backdrop.is-open').forEach(closeModal);
        }
    });

    // Alternância Perfil Usuário / Perfil Administrador (Tela Login)
    const profileTabs = document.querySelectorAll('[data-profile-tab]');
    const perfilInput = document.getElementById('login-perfil');
    const loginSubmitBtn = document.getElementById('login-submit-btn');
    const loginPerfilNote = document.getElementById('login-perfil-note');

    if (profileTabs.length > 0) {
        profileTabs.forEach(tab => {
            tab.addEventListener('click', () => {
                const selected = tab.getAttribute('data-profile-tab');
                profileTabs.forEach(t => {
                    const active = t === tab;
                    t.classList.toggle('is-active', active);
                    t.setAttribute('aria-selected', String(active));
                });

                if (perfilInput) perfilInput.value = selected;
                if (selected === 'admin') {
                    if (loginSubmitBtn) loginSubmitBtn.textContent = 'Entrar como administrador';
                    if (loginPerfilNote) loginPerfilNote.textContent = 'Acesso administrativo para gestão da ONG Patas do Bem.';
                } else {
                    if (loginSubmitBtn) loginSubmitBtn.textContent = 'Entrar como usuário';
                    if (loginPerfilNote) loginPerfilNote.textContent = 'Conta pessoal para adotar, doar e acompanhar.';
                }
            });
        });
    }

    // Submissão: Simular Doação
    const formDoacao = document.getElementById('form-doacao');
    if (formDoacao) {
        formDoacao.addEventListener('submit', async (event) => {
            event.preventDefault();
            const feedback = document.getElementById('doacao-feedback');
            const payload = {
                nome: formDoacao.nome.value,
                email: formDoacao.email.value,
                valor: formDoacao.valor.value,
                formaPagamento: formDoacao.formaPagamento.value,
                campanha: 'Um pote cheio. Muitas vidas felizes.'
            };

            try {
                const res = await fetch('/api/doacoes', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const data = await res.json();
                setFeedback(feedback, `${data.mensagem} (${data.valor} via ${data.formaPagamento} · ${data.dataHora})`, false);
            } catch (err) {
                setFeedback(feedback, 'Não foi possível registrar a simulação agora.', true);
            }
        });
    }

    // Submissão: Login
    const formLogin = document.getElementById('form-login');
    if (formLogin) {
        formLogin.addEventListener('submit', async (event) => {
            event.preventDefault();
            const feedback = document.getElementById('login-feedback');
            const payload = {
                email: formLogin.email.value,
                senha: formLogin.senha.value,
                perfil: formLogin.perfil.value
            };

            try {
                const res = await fetch('/api/auth/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const data = await res.json();
                setFeedback(feedback, data.mensagem, !res.ok);
                if (res.ok) {
                    const destino = payload.perfil === 'admin' ? '/admin' : '/minha-area';
                    setTimeout(() => {
                        window.location.href = destino;
                    }, 450);
                }
            } catch (err) {
                setFeedback(feedback, 'Erro ao realizar login.', true);
            }
        });
    }

    // Submissão: Registrar-se
    const formRegistrar = document.getElementById('form-registrar');
    if (formRegistrar) {
        formRegistrar.addEventListener('submit', async (event) => {
            event.preventDefault();
            const feedback = document.getElementById('registrar-feedback');
            const payload = {
                nome: formRegistrar.nome.value,
                email: formRegistrar.email.value,
                senha: formRegistrar.senha.value,
                confirmarSenha: formRegistrar.confirmarSenha.value,
                termos: formRegistrar.termos.checked
            };

            try {
                const res = await fetch('/api/auth/registrar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const data = await res.json();
                setFeedback(feedback, data.mensagem, !res.ok);
            } catch (err) {
                setFeedback(feedback, 'Erro ao criar conta.', true);
            }
        });
    }

    // Submissão: Recuperar Acesso
    const formRecuperar = document.getElementById('form-recuperar');
    if (formRecuperar) {
        formRecuperar.addEventListener('submit', async (event) => {
            event.preventDefault();
            const feedback = document.getElementById('recuperar-feedback');
            const payload = { email: formRecuperar.email.value };

            try {
                const res = await fetch('/api/auth/recuperar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const data = await res.json();
                if (res.ok) {
                    feedback.innerHTML = `${data.mensagem} <a href="/redefinir-senha" style="text-decoration:underline;font-weight:700;margin-left:6px;">Abrir redefinição de senha →</a>`;
                    feedback.classList.remove('is-error');
                    feedback.classList.add('is-visible');
                } else {
                    setFeedback(feedback, data.mensagem, true);
                }
            } catch (err) {
                setFeedback(feedback, 'Erro ao enviar instruções.', true);
            }
        });
    }

    // Submissão: Redefinir Senha
    const formRedefinir = document.getElementById('form-redefinir');
    if (formRedefinir) {
        formRedefinir.addEventListener('submit', async (event) => {
            event.preventDefault();
            const feedback = document.getElementById('redefinir-feedback');
            const payload = {
                novaSenha: formRedefinir.novaSenha.value,
                confirmarNovaSenha: formRedefinir.confirmarNovaSenha.value
            };

            try {
                const res = await fetch('/api/auth/redefinir', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const data = await res.json();
                setFeedback(feedback, data.mensagem, !res.ok);
            } catch (err) {
                setFeedback(feedback, 'Erro ao salvar nova senha.', true);
            }
        });
    }
});

function setFeedback(el, message, isError) {
    if (!el) return;
    el.textContent = message;
    el.classList.toggle('is-error', Boolean(isError));
    el.classList.add('is-visible');
}
