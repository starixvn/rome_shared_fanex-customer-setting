import { useRouter } from "next/router";
import { useDispatch } from "react-redux";
import { useLoginMutation } from "../features/auth/authApi";
import { setCredentials } from "../features/auth/authSlide";

export default function Login() {
  const router = useRouter();
  const dispatch = useDispatch();
  const [login] = useLoginMutation();
  return (
    <div>
      <button
        onClick={async () => {
          var response = await login({
            userId: 12345,
            username: "alvin",
            roleId: 4,
            subAccId: 0,
          }).unwrap();

          dispatch(setCredentials(response));
          router.push("/");
        }}
      ></button>
    </div>
  );
}
