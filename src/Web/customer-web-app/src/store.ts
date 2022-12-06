import { configureStore, ConfigureStoreOptions } from "@reduxjs/toolkit";
import { createWrapper } from "next-redux-wrapper";
import { authApi } from "./features/auth/authApi";
import { authSlice } from "./features/auth/authSlide";
import { customerApi } from "./features/customer/customerApi";

//import counterReducer from "./features/counter/counterSlice";

// create your reducer

export const makeStore = (
  options?: ConfigureStoreOptions["preloadedState"] | undefined
) =>
  configureStore({
    reducer: {
      [authApi.reducerPath]: authApi.reducer,
      [customerApi.reducerPath]: customerApi.reducer,
      auth: authSlice.reducer,
    },
    devTools: true,
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat([
        authApi.middleware,
        customerApi.middleware,
      ]),
    ...options,
  });

export type AppStore = ReturnType<typeof makeStore>;
export type AppState = ReturnType<AppStore["getState"]>;
export type AppDispatch = AppStore["dispatch"];

export const wrapper = createWrapper(makeStore);
