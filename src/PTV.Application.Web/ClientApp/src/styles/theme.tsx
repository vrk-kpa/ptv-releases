import { Theme as MUITheme } from '@mui/system';
import { SuomifiTheme, defaultSuomifiTheme } from 'suomifi-ui-components';

// See https://material-ui.com/guides/typescript/

interface PTVTheme {
  layout: {
    maxWidth: number;
  };
  aside: {
    background: string;
    border: string;
  };
  colors: {
    draft: string;
    published: string;
    archived: string;
    modified: string;
    pageBackground: string;
    boxBackground: string;
    border: string;
    error: string;
    info: string;
    link: string;
    hint: string;
    notificationInfo: string;
    notificationWarning: string;
    errorUnderline: string;
    errorHighlight: string;
  };
  absoluteFocus: {
    content: string;
    position: string;
    pointerEvents: string;
    top: string;
    right: string;
    bottom: string;
    left: string;
    borderRadius: string;
    backgroundColor: string;
    border: string;
    boxSizing: string;
    boxShadow: string;
    zIndex: string;
  };
  suomifi: SuomifiTheme;
}

interface PTVThemeOptions {
  layout?: {
    maxWidth?: number;
  };
  aside?: {
    background?: string;
    border?: string;
  };
  colors?: {
    draft?: string;
    published?: string;
    archived?: string;
    modified?: string;
    pageBackground?: string;
    boxBackground?: string;
    border?: string;
    error?: string;
    info?: string;
    link?: string;
    hint: string;
    notificationInfo?: string;
    notificationWarning?: string;
    errorUnderline?: string;
    errorHighlight?: string;
  };
  absoluteFocus?: {
    content: string;
    position: string;
    pointerEvents: string;
    top: string;
    right: string;
    bottom: string;
    left: string;
    borderRadius: string;
    backgroundColor: string;
    border: string;
    boxSizing: string;
    boxShadow: string;
    zIndex: string;
  };
  suomifi?: SuomifiTheme;
}

declare module '@mui/styles' {
  export interface DefaultTheme extends MUITheme, PTVTheme {}
  // allow configuration using `createMuiTheme`
  export type ThemeOptions = PTVThemeOptions;
}

export const themeOptions = {
  components: {
    MuiCssBaseline: {
      styleOverrides: {},
    },
  },
  spacing: 10,
  palette: {
    background: {
      default: '#F6F6F7',
    },
    primary: {
      main: 'rgb(42, 110, 187)',
    },
  },
  layout: {
    maxWidth: 2040,
  },
  aside: {
    background: 'rgb(254, 254, 254)',
    border: '1px solid rgb(201, 205, 207)',
  },
  colors: {
    draft: '#EA7125',
    published: '#00B38A',
    archived: '#A5ACB0',
    modified: '#EA7125',
    pageBackground: '#F6F6F7',
    boxBackground: 'rgb(254, 254, 254)',
    border: 'rgb(201, 205, 207)',
    error: 'rgb(195, 57, 50)',
    info: 'rgb(6, 132, 102)',
    link: 'rgb(42, 110, 187)',
    hint: '#2a6ebb',
    notificationInfo: 'rgb(241, 250, 253)',
    notificationWarning: 'rgb(255, 246, 224)',
    errorUnderline: 'rgb(232, 106, 28)',
    errorHighlight: 'rgb(255, 246, 224)',
  },
  typography: {
    fontFamily: '"Source Sans Pro", "Helvetica Neue", Arial, sans-serif',
    fontWeightRegular: 'normal',
    fontSize: 18,
    allVariants: {
      color: 'rgb(41, 41, 41)',
      lineHeight: '1.5',
    },
    // If possible use Paragraph, Text and Heading component from suomi-fi
    // instead of h1, etc from material ui
  },
  // Absolute focus styles from suomifi-theme do not work with Emotion.
  absoluteFocus: {
    content: '""',
    position: 'absolute',
    pointerEvents: 'none',
    top: '-2px',
    right: '-2px',
    bottom: '-2px',
    left: '-2px',
    borderRadius: defaultSuomifiTheme.radius.focus,
    backgroundColor: 'transparent',
    border: `0px solid ${defaultSuomifiTheme.colors.whiteBase}`,
    boxSizing: 'border-box',
    boxShadow: `0 0 0 2px ${defaultSuomifiTheme.colors.accentSecondary}`,
    zIndex: defaultSuomifiTheme.zindexes.focus,
  },
  suomifi: defaultSuomifiTheme,
};

declare module '@mui/material/styles' {
  export interface Theme extends MUITheme, PTVTheme {}

  export type ThemeOptions = PTVThemeOptions;
}
