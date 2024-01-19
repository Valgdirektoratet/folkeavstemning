export const makeFirstLetterUpperCase = (word: string) => {
    return word[0].toUpperCase() + word.slice(1).toLowerCase();
}

export const makeFirstLetterLowerCase = (word: string) => {
    return word[0].toLowerCase() + word.slice(1);
}