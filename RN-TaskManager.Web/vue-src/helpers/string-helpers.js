/**
 * Аббревиатура по первым букам текста
 * @param {String} t Текст
 */
export function abbreviation(t) {
    return t.split(' ').map(e => e[0]).join('').toUpperCase();
}