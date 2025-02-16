/** @type {import('tailwindcss').Config} */
module.exports = {
    prefix: 'tw-',
    content: [
        "./Components/**/*.{razor,html,cshtml}",
        "../UbikLink.Common.RazorUI/**/*.razor",
        "../UbikLink.Security.UI.Client/**/*.razor"
    ],
    safelist: [
        'tw-hidden',
        'tw-block'
    ],
    theme: {
        extend: {},
    },
    plugins: [],
}
