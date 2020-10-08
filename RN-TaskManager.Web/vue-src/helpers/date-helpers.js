/**
 * Получение из строки даты в локальном формате
 * @param {String} d Дата в строчном формате
 */
export function localeDateFormat(d) {

    if (d === undefined || d === null || d === '')
        return '';

    return new Date(Date.parse(d)).toLocaleDateString();
}


/**
 * Получение из строки даты в ISO формате
 * @param {String} d Дата в строчном формате
 */
export function ISODateFormat(d) {

    if (d == undefined || d == null || d === '')
        return '';

    return new Date(Date.parse(d)).toISOString();
}