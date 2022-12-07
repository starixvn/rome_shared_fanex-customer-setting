import { useSelector } from "react-redux";
import { selectAuth } from "./authSlide";
import Unauthorized from "./Unauthorize";

type Props = {
  readonly children?: React.ReactNode;
};

export const AuthGuard: React.FC<Props> = ({ children }) => {
  const { accessToken } = useSelector(selectAuth);
  if (accessToken) {
    return <>{children}</>;
  }

  return <Unauthorized />;
};
