/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    container: {
      center: true,
    },
    screens: {
      'xs': '475px',
      ...defaultTheme.screens,
    },
    listStyleType: {
      square: 'square'
    },
    fontSize: {
      ...defaultTheme.fontSize,
      sm: ['0.875rem', '1.5rem'],
    },
    extend: {
      transitionProperty: {
        'max-height': 'max-height'
      },
      colors: {
        'primary': {
          100: '#F5F6F7',
          200: '#E2E5E7',
          300: '#D9DCDF',
          400: '#C5CACF',
          500: '#405061',
          600: '#2E3949',
          700: '#1D2232',
          800: '#0D0C1A',
          900: '#030307'
        },
        'orange': {
          25: '#FEF7F2',
          50: '#FDEFE5',
          100: '#FCE7D9',
          200: '#FADFCC',
          300: '#F8CFB2',
          500: '#E66300',
        },
      },

    },
  },
  plugins: [],
}

