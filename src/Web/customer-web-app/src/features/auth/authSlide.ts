import {
  createAsyncThunk,
  createSlice,
  SerializedError,
} from "@reduxjs/toolkit";
import { sso } from "./authApi";

export enum AuthStatus {
  IDLE = "idle",
  LOADING = "loading",
}

export interface AuthState {
  accessToken: string;
  status: AuthStatus;
  error?: SerializedError;
}

const initialState: AuthState = {
  accessToken: "",
  status: AuthStatus.IDLE,
  error: undefined,
};

export const ssoAsync = createAsyncThunk("auth/sso", async () => {
  const response = await sso();

  return response;
});

export const authSlice = createSlice({
  name: "auth",
  initialState: initialState,
  reducers: {
    setCredentials: (state, action) => {
      state.accessToken = action.payload;
    },
    logout: () => initialState,
  },
  extraReducers: (builder) => {
    builder
      .addCase(ssoAsync.pending, (state) => {
        state.status = AuthStatus.LOADING;
      })
      .addCase(ssoAsync.fulfilled, (state, action) => {
        state.status = AuthStatus.IDLE;
        state.accessToken = action.payload;
      });
  },
});

export const { setCredentials, logout } = authSlice.actions;
