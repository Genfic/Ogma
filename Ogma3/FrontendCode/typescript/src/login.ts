import { GetApiSignin as getSignInData } from "@g/paths-public";
import { $id } from "@h/dom";

const avatar = $id<HTMLImageElement>("user-avatar");
const title = $id("user-title");
const name = $id<HTMLInputElement>("Input_Name");

name.addEventListener("focusout", async (e) => {
	const target = e.target as HTMLInputElement;

	const res = await getSignInData(target.value);
	if (!res.ok) throw res.error;

	avatar.src = res.data.avatar;
	title.innerText = res.data.title ?? "";
});
