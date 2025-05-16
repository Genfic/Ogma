import { createSignal } from "solid-js";
import { customElement, type ComponentType } from "solid-element";
import type { Empty } from "@t/utils";
import sharedCss from "./shared.css";
import css from "./password-input.css";
import { Styled } from "./common/_styled";

const PasswordInput: ComponentType<Empty> = (_, { element }) => {
	if (!(element instanceof Element)) throw Error("Not an element?");
	const input = element.previousElementSibling as HTMLInputElement;

	const [showPassword, setShowPassword] = createSignal(false);

	const toggle = () => {
		setShowPassword(!showPassword());
		input.type = showPassword() ? "text" : "password";
	};

	const icon = (): string => (showPassword() ? "lucide:eye" : "lucide:eye-closed");

	return (
		<button type="button" class="action-btn show-password" onClick={toggle}>
			<o-icon icon={icon()} />
		</button>
	);
};

customElement("o-password-toggle", {}, Styled(PasswordInput, sharedCss, css));
