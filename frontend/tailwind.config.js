/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      colors: {
        brand: {
          DEFAULT: '#34d399',
          soft: '#6ee7b7',
          deep: '#059669',
        },
        surface: {
          950: '#0a0f1a',
          900: '#0f1626',
          800: '#182135',
          700: '#243049',
        },
      },
      boxShadow: {
        glow: '0 0 40px -12px rgba(52, 211, 153, 0.35)',
      },
    },
  },
  plugins: [],
};
