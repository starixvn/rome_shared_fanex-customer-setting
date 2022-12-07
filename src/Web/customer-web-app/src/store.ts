import { configureStore, ThunkAction } from "@reduxjs/toolkit";
import { createWrapper } from "next-redux-wrapper";
import { Action } from "redux";
import { AuthSlide } from "./features/auth/authSlide";
import { customerApi } from "./features/customer/customerApi";

const makeStore = () =>
  configureStore({
    reducer: {
      auth: AuthSlide.reducer,
      [customerApi.reducerPath]: customerApi.reducer,
    },
    middleware: (getDefaultMiddleware) =>
      getDefaultMiddleware().concat(customerApi.middleware),
    devTools: true,
  });

export type AppStore = ReturnType<typeof makeStore>;
export type AppState = ReturnType<AppStore["getState"]>;
export type AppDispatch = AppStore["dispatch"];
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  AppState,
  unknown,
  Action
>;

export const wrapper = createWrapper<AppStore>(makeStore);
