/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Areas/Admin/Views/**/*.cshtml",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        admin: {
          primary: '#1e293b', // slate-800
          secondary: '#334155', // slate-700
          accent: '#0f172a', // slate-900
        }
      }
    },
  },
  plugins: [],
}
