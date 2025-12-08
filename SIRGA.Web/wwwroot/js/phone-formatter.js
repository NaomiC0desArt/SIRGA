function formatDominicanPhone(input) {
    let phone = input.replace(/\D/g, '');
    phone = phone.substring(0, 10);

    if (phone.length <= 3) {
        return phone;
    } else if (phone.length <= 6) {
        return phone.slice(0, 3) + '-' + phone.slice(3);
    } else {
        return phone.slice(0, 3) + '-' + phone.slice(3, 6) + '-' + phone.slice(6);
    }
}

// Validar formato de teléfono RD (809, 829, 849)
function validateDominicanPhone(phoneNumber) {
    const cleaned = phoneNumber.replace(/-/g, '');
    if (cleaned.length !== 10) return false;
    const areaCode = cleaned.substring(0, 3);
    return ['809', '829', '849'].includes(areaCode);
}

// Mostrar mensaje de error
function showPhoneError(inputElement, message) {
    // Remover error anterior si existe
    removePhoneError(inputElement);

    // Cambiar borde a rojo
    inputElement.classList.add('border-red-500', 'focus:ring-red-500', 'focus:border-red-500');
    inputElement.classList.remove('border-gray-300', 'focus:ring-blue-500', 'focus:border-blue-500', 'focus:ring-purple-500', 'focus:border-purple-500');

    // Crear mensaje de error
    const errorMsg = document.createElement('p');
    errorMsg.className = 'phone-error-msg text-xs text-red-500 mt-1 flex items-start';
    errorMsg.innerHTML = `
        <svg class="w-4 h-4 mr-1 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
        </svg>
        <span>${message}</span>
    `;

    // Insertar después del input
    inputElement.parentNode.insertBefore(errorMsg, inputElement.nextSibling);
}

// Remover mensaje de error
function removePhoneError(inputElement) {
    // Restaurar borde normal
    inputElement.classList.remove('border-red-500', 'focus:ring-red-500', 'focus:border-red-500');
    inputElement.classList.add('border-gray-300');

    // Remover mensaje de error
    const errorMsg = inputElement.parentNode.querySelector('.phone-error-msg');
    if (errorMsg) {
        errorMsg.remove();
    }
}

// Mostrar mensaje de éxito (opcional)
function showPhoneSuccess(inputElement) {
    removePhoneError(inputElement);

    // Cambiar borde a verde brevemente
    inputElement.classList.add('border-green-500', 'focus:ring-green-500', 'focus:border-green-500');
    inputElement.classList.remove('border-gray-300', 'border-red-500');

    // Volver a normal después de 1 segundo
    setTimeout(() => {
        inputElement.classList.remove('border-green-500', 'focus:ring-green-500', 'focus:border-green-500');
        inputElement.classList.add('border-gray-300');
    }, 1000);
}

// Aplicar formateo con corrección de cursor
function applyPhoneFormatting(inputElement) {
    if (!inputElement) return;

    inputElement.addEventListener('input', function (e) {
        // Guardar posición del cursor y valor anterior
        const cursorPos = this.selectionStart;
        const oldValue = this.value;
        const oldLength = oldValue.length;

        // Contar cuántos guiones había antes del cursor
        const guionesAntes = (oldValue.substring(0, cursorPos).match(/-/g) || []).length;

        // Formatear el nuevo valor
        const newValue = formatDominicanPhone(this.value);
        this.value = newValue;

        // Contar cuántos guiones hay ahora antes de la posición del cursor
        const guionesDespues = (newValue.substring(0, cursorPos).match(/-/g) || []).length;

        // Calcular la diferencia de guiones
        const diff = guionesDespues - guionesAntes;

        // Ajustar la posición del cursor
        let newCursorPos = cursorPos + diff;

        // Si el cursor está justo después de un guion, moverlo después del guion
        if (newValue[newCursorPos - 1] === '-') {
            newCursorPos++;
        }

        // Asegurar que el cursor no se salga del rango
        newCursorPos = Math.max(0, Math.min(newCursorPos, newValue.length));

        // Establecer la nueva posición del cursor
        this.setSelectionRange(newCursorPos, newCursorPos);

        // Remover error mientras escribe
        if (this.value.length < 12) { // 12 = 10 dígitos + 2 guiones
            removePhoneError(this);
        }
    });

    // Validar cuando el usuario termina de escribir (pierde el foco)
    inputElement.addEventListener('blur', function () {
        if (!this.value) {
            removePhoneError(this);
            return;
        }

        const cleaned = this.value.replace(/-/g, '');

        // Validar longitud
        if (cleaned.length > 0 && cleaned.length < 10) {
            showPhoneError(this, 'El teléfono debe tener 10 dígitos. Ej: 809-459-2688');
            return;
        }

        // Validar código de área
        if (cleaned.length === 10 && !validateDominicanPhone(this.value)) {
            const areaCode = cleaned.substring(0, 3);
            showPhoneError(this, `El código de área "${areaCode}" no es válido. Use: 809, 829 o 849`);
            return;
        }

        // Si todo está bien
        if (cleaned.length === 10) {
            showPhoneSuccess(this);
        }
    });

    // Validar al enfocar (para mostrar errores previos)
    inputElement.addEventListener('focus', function () {
        // Si hay error, no hacer nada (mantener el error visible)
        if (this.classList.contains('border-red-500')) {
            return;
        }
    });

    // Formatear valor inicial si existe
    if (inputElement.value) {
        inputElement.value = formatDominicanPhone(inputElement.value);
    }
}

// Inicializar automáticamente todos los campos de teléfono
function initializePhoneFormatters() {
    // Buscar todos los inputs con atributo data-phone-format
    const phoneInputs = document.querySelectorAll('[data-phone-format="dominican"]');

    phoneInputs.forEach(input => {
        applyPhoneFormatting(input);
    });

    // También buscar por name o id que contengan "phone" o "telefono"
    const autoDetectInputs = document.querySelectorAll(
        'input[name*="Phone"], input[name*="Telefono"], ' +
        'input[id*="Phone"], input[id*="Telefono"], ' +
        'input[type="tel"]'
    );

    autoDetectInputs.forEach(input => {
        // Solo aplicar si no tiene el atributo data-phone-format="false"
        if (input.getAttribute('data-phone-format') !== 'false') {
            applyPhoneFormatting(input);
        }
    });
}

// Inicializar cuando el DOM esté listo
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializePhoneFormatters);
} else {
    initializePhoneFormatters();
}

// Validación de formulario antes de enviar (opcional)
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            let hasPhoneErrors = false;

            // Buscar todos los campos de teléfono en el formulario
            const phoneInputs = this.querySelectorAll(
                'input[name*="Phone"], input[name*="Telefono"], input[type="tel"]'
            );

            phoneInputs.forEach(input => {
                if (input.getAttribute('data-phone-format') === 'false') return;

                const cleaned = input.value.replace(/-/g, '');

                // Si el campo tiene valor, validarlo
                if (cleaned.length > 0) {
                    if (cleaned.length !== 10) {
                        showPhoneError(input, 'El teléfono debe tener 10 dígitos');
                        hasPhoneErrors = true;
                    } else if (!validateDominicanPhone(input.value)) {
                        const areaCode = cleaned.substring(0, 3);
                        showPhoneError(input, `El código de área "${areaCode}" no es válido. Use: 809, 829 o 849`);
                        hasPhoneErrors = true;
                    }
                }
            });

            // Si hay errores, prevenir el envío y hacer scroll al primer error
            if (hasPhoneErrors) {
                e.preventDefault();

                const firstError = this.querySelector('.phone-error-msg');
                if (firstError) {
                    firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                }

                // Mostrar notificación general
                alert('Por favor, corrija los errores en los números de teléfono antes de continuar.');
            }
        });
    });
});

// Exportar funciones para uso manual si es necesario
window.formatDominicanPhone = formatDominicanPhone;
window.validateDominicanPhone = validateDominicanPhone;
window.applyPhoneFormatting = applyPhoneFormatting;
window.initializePhoneFormatters = initializePhoneFormatters;