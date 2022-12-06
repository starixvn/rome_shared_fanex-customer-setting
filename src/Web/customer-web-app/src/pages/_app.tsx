import "../styles/globals.css";
import React, { FC } from "react";
import { AppProps } from "next/app";
import { wrapper } from "../store";
import { Provider } from "react-redux";
import { CookiesProvider } from "react-cookie";

const MyApp: FC<AppProps> = ({ Component, ...rest }) => {
  const { store, props } = wrapper.useWrappedStore(rest);
  return (
    <Provider store={store}>
      <CookiesProvider>
        <Component {...props.pageProps} />
      </CookiesProvider>
    </Provider>
  );
};

export default MyApp;
