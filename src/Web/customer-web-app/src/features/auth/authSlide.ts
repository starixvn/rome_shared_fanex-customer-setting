import { createSlice } from "@reduxjs/toolkit";
import { HYDRATE } from "next-redux-wrapper";
import { AppState } from "../../store";

export const AuthSlide = createSlice({
  name: "auth",
  initialState: { accessToken: null },
  reducers: {
    setAccessToken: (state, action) => {
      state.accessToken = action.payload;
    },
    reset: (state) => {
      state.accessToken = null;
    },
  },
  extraReducers: {
    [HYDRATE]: (state, action) => {
      console.log("HYDRATE", action.payload);

      if (!action.payload.auth.accessToken) {
        return state;
      }

      state.accessToken = action.payload.auth.accessToken;
    },
  },
});

export const { setAccessToken, reset } = AuthSlide.actions;

export const selectAuth = (state: AppState) => state.auth;
