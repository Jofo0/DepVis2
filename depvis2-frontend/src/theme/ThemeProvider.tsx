// theme/ThemeProvider.tsx
import { createContext, useContext, useEffect, useState } from "react";

const ThemeContext = createContext({
  darkMode: false,
  toggle: () => {},
});

export const ThemeProvider = ({ children }: { children: React.ReactNode }) => {
  const [darkMode, setDarkMode] = useState(false);

  useEffect(() => {
    const stored = localStorage.getItem("theme");
    if (stored) setDarkMode(stored === "dark");
  }, []);

  useEffect(() => {
    const root = document.documentElement;
    if (darkMode) {
      root.classList.add("dark");
      localStorage.setItem("theme", "dark");
    } else {
      root.classList.remove("dark");
      localStorage.setItem("theme", "light");
    }
  }, [darkMode]);

  return (
    <ThemeContext.Provider
      value={{
        darkMode,
        toggle: () => setDarkMode((d) => !d),
      }}
    >
      {children}
    </ThemeContext.Provider>
  );
};

export const useTheme = () => {
  return useContext(ThemeContext);
};
