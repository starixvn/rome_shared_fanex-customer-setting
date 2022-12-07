import {
  BaseQueryApi,
  createApi,
  FetchArgs,
  fetchBaseQuery,
} from "@reduxjs/toolkit/query/react";
import { HYDRATE } from "next-redux-wrapper";
import { AppState } from "../../store";
import { setAccessToken, reset } from "../auth/authSlide";

const CUSTOMER_URL = "http://localhost:5270";

export interface WeatherForeCast {
  date: Date;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

const baseQuery = fetchBaseQuery({
  baseUrl: CUSTOMER_URL,
  credentials: "include",
  prepareHeaders: (headers, { getState }) => {
    const accessToken = (getState() as AppState).auth.accessToken;
    if (accessToken) {
      headers.set("Authorization", `bearer ${accessToken}`);
    }
    return headers;
  },
});

const baseQueryWithReAuth = async (
  args: string | FetchArgs,
  api: BaseQueryApi,
  extraOptions: {}
) => {
  let result = await baseQuery(args, api, extraOptions);

  if (result?.error?.status === 403) {
    console.log("sending refresh token");
    // send refresh token to get new access token
    const refreshResult = await baseQuery("/refresh-token", api, extraOptions);
    console.log(refreshResult);
    if (refreshResult?.data) {
      // store the new token
      api.dispatch(setAccessToken(refreshResult.data));
      // retry the original query with new access token
      result = await baseQuery(args, api, extraOptions);
    } else {
      api.dispatch(reset());
    }
  }

  return result;
};

export const customerApi = createApi({
  reducerPath: "customerApi",
  baseQuery: baseQueryWithReAuth,
  extractRehydrationInfo(action, { reducerPath }) {
    if (action.type === HYDRATE) {
      return action.payload[reducerPath];
    }
  },
  endpoints: (builder) => ({
    getWeatherForecast: builder.query<WeatherForeCast[], void>({
      query: () => ({ url: "WeatherForecast" }),
    }),
  }),
});

export const {
  useGetWeatherForecastQuery,
  util: { getRunningQueriesThunk },
} = customerApi;

export const { getWeatherForecast } = customerApi.endpoints;
