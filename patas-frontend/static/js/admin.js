document.addEventListener('DOMContentLoaded', () => {
    const popovers = document.querySelectorAll('.admin-popover');

    function closeAllPopovers() {
        popovers.forEach(p => p.classList.remove('is-open'));
    }

    document.querySelectorAll('[data-toggle-popover]').forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.stopPropagation();
            const id = btn.getAttribute('data-toggle-popover');
            const target = document.getElementById(id);
            const wasOpen = target && target.classList.contains('is-open');
            closeAllPopovers();
            if (target && !wasOpen) {
                target.classList.add('is-open');
            }
        });
    });

    popovers.forEach(p => {
        p.addEventListener('click', (e) => e.stopPropagation());
    });

    document.addEventListener('click', closeAllPopovers);

    // Busca global interativa
    const searchInput = document.getElementById('admin-global-search');
    const searchPopover = document.getElementById('popover-busca');
    const searchResultsEl = document.getElementById('search-results-list');

    const quickItems = [
        { label: 'Luna · #AN-0128 (Canina, Disponível)', url: '/admin/animais/luna/prontuario' },
        { label: 'Thor · #AN-0127 (Canino, Em tratamento)', url: '/admin/animais/thor/prontuario' },
        { label: 'Ana Souza · Luna (#AD-0048)', url: '/admin/adocoes/AD-0048/analisar' },
        { label: 'Vacina múltipla · Luna (09:00)', url: '/admin/saude' },
        { label: 'Campanha: Um pote cheio', url: '/admin/financeiro/campanha' }
    ];

    function renderSearch(query) {
        if (!searchResultsEl) return;
        const q = (query || '').trim().toLowerCase();
        const filtered = q
            ? quickItems.filter(i => i.label.toLowerCase().includes(q))
            : quickItems.slice(0, 3);

        searchResultsEl.innerHTML = filtered.length
            ? filtered.map(i => `<a href="${i.url}" class="search-result-item"><span>${i.label}</span><span style="color:#237463;font-weight:600;">Abrir →</span></a>`).join('')
            : '<p style="font-size:12px;color:#7B8883;padding:8px;">Nenhum registro encontrado.</p>';
    }

    if (searchInput && searchPopover) {
        searchInput.addEventListener('focus', (e) => {
            e.stopPropagation();
            closeAllPopovers();
            renderSearch(searchInput.value);
            searchPopover.classList.add('is-open');
        });
        searchInput.addEventListener('input', () => {
            renderSearch(searchInput.value);
            searchPopover.classList.add('is-open');
        });
    }

    // Marcar notificações como lidas
    const markReadBtn = document.getElementById('btn-mark-notifs-read');
    if (markReadBtn) {
        markReadBtn.addEventListener('click', () => {
            document.querySelectorAll('#popover-notificacoes .notif-item').forEach(item => {
                item.classList.add('is-read');
            });
            const bellBadge = document.getElementById('bell-unread-count');
            const popBadge = document.getElementById('popover-unread-count');
            if (bellBadge) bellBadge.textContent = '0';
            if (popBadge) popBadge.textContent = '0';
        });
    }

    // Filtro de animais (A02 · Animais)
    const filterNome = document.getElementById('buscar-animal-nome');
    const filterEspecie = document.getElementById('filter-especie');
    const filterSituacao = document.getElementById('filter-situacao');
    const btnAplicarFiltros = document.getElementById('btn-aplicar-filtros');
    const animalRows = document.querySelectorAll('[data-animal-row]');

    function applyAnimalFilters() {
        const q = filterNome ? filterNome.value.trim().toLowerCase() : '';
        const esp = filterEspecie ? filterEspecie.value.toLowerCase() : 'todas';
        const sit = filterSituacao ? filterSituacao.value.toLowerCase() : 'todas';

        animalRows.forEach(row => {
            const rowNome = (row.getAttribute('data-nome') || '').toLowerCase();
            const rowEsp = (row.getAttribute('data-especie') || '').toLowerCase();
            const rowSit = (row.getAttribute('data-situacao') || '').toLowerCase();
            const matchNome = !q || rowNome.includes(q);
            const matchEsp = esp === 'todas' || rowEsp.includes(esp);
            const matchSit = sit === 'todas' || rowSit === sit;
            row.style.display = (matchNome && matchEsp && matchSit) ? '' : 'none';
        });
    }

    if (filterNome) filterNome.addEventListener('input', applyAnimalFilters);
    if (filterEspecie) filterEspecie.addEventListener('change', applyAnimalFilters);
    if (filterSituacao) filterSituacao.addEventListener('change', applyAnimalFilters);
    if (btnAplicarFiltros) btnAplicarFiltros.addEventListener('click', applyAnimalFilters);

    // Filtro de busca interna em Voluntários (A06 · Voluntários)
    const volSearch = document.getElementById('buscar-voluntario');
    const volRows = document.querySelectorAll('[data-voluntario-row]');
    if (volSearch && volRows.length > 0) {
        volSearch.addEventListener('input', () => {
            const q = volSearch.value.trim().toLowerCase();
            volRows.forEach(row => {
                const text = (row.getAttribute('data-busca') || '').toLowerCase();
                row.style.display = (!q || text.includes(q)) ? '' : 'none';
            });
        });
    }

    // Seleção de período (A01 · Visão geral)
    document.querySelectorAll('[data-period-option]').forEach(opt => {
        opt.addEventListener('click', () => {
            const label = opt.getAttribute('data-period-option');
            const currentEl = document.getElementById('current-period-label');
            if (currentEl && label) currentEl.textContent = label;
            closeAllPopovers();
        });
    });

    // Upload e preview de foto do animal (A09 · Cadastrar/Editar animal)
    const animalPhotoInput = document.getElementById('animal-photo-input');
    const animalPhotoDropzone = document.getElementById('animal-photo-dropzone');
    const animalPhotoPreview = document.getElementById('animal-photo-preview');
    const animalPhotoFilename = document.getElementById('animal-photo-filename');

    function handleAnimalPhotoFile(file) {
        if (!file) return;
        if (file.size > 5 * 1024 * 1024) {
            if (animalPhotoFilename) {
                animalPhotoFilename.textContent = 'Arquivo maior que 5 MB. Escolha outra imagem.';
                animalPhotoFilename.style.color = '#C43E42';
            }
            return;
        }
        const reader = new FileReader();
        reader.onload = (e) => {
            if (animalPhotoPreview && e.target && e.target.result) {
                animalPhotoPreview.src = e.target.result;
            }
            if (animalPhotoFilename) {
                animalPhotoFilename.textContent = `${file.name} · foto selecionada`;
                animalPhotoFilename.style.color = '#237463';
            }
        };
        reader.readAsDataURL(file);
    }

    if (animalPhotoInput) {
        animalPhotoInput.addEventListener('change', (e) => {
            const file = e.target.files && e.target.files[0];
            handleAnimalPhotoFile(file);
        });
    }

    if (animalPhotoDropzone) {
        ['dragenter', 'dragover'].forEach(evt => {
            animalPhotoDropzone.addEventListener(evt, (e) => {
                e.preventDefault();
                animalPhotoDropzone.style.borderColor = '#237463';
                animalPhotoDropzone.style.backgroundColor = '#F4F9F6';
            });
        });
        ['dragleave', 'drop'].forEach(evt => {
            animalPhotoDropzone.addEventListener(evt, (e) => {
                e.preventDefault();
                animalPhotoDropzone.style.borderColor = '#B9CEC4';
                animalPhotoDropzone.style.backgroundColor = '#FFFFFF';
            });
        });
        animalPhotoDropzone.addEventListener('drop', (e) => {
            const file = e.dataTransfer && e.dataTransfer.files && e.dataTransfer.files[0];
            handleAnimalPhotoFile(file);
        });
    }

    // Upload e preview de avatar da administradora (Meu Perfil)
    const adminAvatarInput = document.getElementById('admin-avatar-input');
    const modalAvatarPreview = document.getElementById('modal-avatar-preview');
    const topbarAvatar = document.querySelector('.admin-user-btn .admin-avatar-svg');

    if (adminAvatarInput) {
        adminAvatarInput.addEventListener('change', (e) => {
            const file = e.target.files && e.target.files[0];
            if (!file) return;
            const reader = new FileReader();
            reader.onload = (ev) => {
                const src = ev.target && ev.target.result;
                if (!src) return;
                [modalAvatarPreview, topbarAvatar].forEach(el => {
                    if (el) {
                        el.innerHTML = `<img src="${src}" alt="Foto de perfil" style="width:100%;height:100%;object-fit:cover;border-radius:50%;display:block;">`;
                    }
                });
            };
            reader.readAsDataURL(file);
        });
    }
});
