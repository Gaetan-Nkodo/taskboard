/** @type {import('tailwindcss').Config} */
export default {
  darkMode: "class",
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}"
  ],
  theme: {
    extend: {
      colors: {
        // Palette claire
        background: "#f8fafc",
        surface: "#ffffff",
        primary: "#3b82f6",
        primaryDark: "#1e40af",
        secondary: "#64748b",
        danger: "#ef4444",

        // Palette sombre
        darkBackground: "#0d0f12",
        darkSurface: "#1a1d21",
        darkText: "#f5f5f5",
        darkBorder: "#2a2d31",
      },

      borderRadius: {
        sm: "6px",
        md: "10px",
        lg: "14px",
      },

      boxShadow: {
        card: "0 4px 12px rgba(0,0,0,0.08)",
        modal: "0 8px 24px rgba(0,0,0,0.12)",
      },

      keyframes: {
        fadeIn: {
          "0%": { opacity: 0, transform: "translateY(10px)" },
          "100%": { opacity: 1, transform: "translateY(0)" },
        },
      },

      animation: {
        fadeIn: "fadeIn 0.4s ease-out",
      },
    },
  },
  plugins: []
}
