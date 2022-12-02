import React from "react";
import { useAppSelector } from "../../hook";
import { AuthStatus } from "./authSlide";
import Unauthorized from "./Unauthorize";

export const AuthGuard: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { status, accessToken } = useAppSelector((state) => state.auth);

  if (status === AuthStatus.LOADING) {
    return <>loading...</>;
  }

  if (accessToken === "") {
    return <Unauthorized />;
  }

  return <>{children}</>;
};
