# CSRF Tokens

Many API endpoints are protected with antiforgery (X-CSRF) tokens. Rendering that token
on the page, for use with JS, can be done in two ways.

## Token element

```cs
Html.AntiForgeryToken();
```

The above code will generate an `<input>` element that contains the token. For example

```html
<input
	name="__RequestVerificationToken"
	type="hidden"
	value="[TOKEN]">
```

Example usage would be

```html
@Html.AntiForgeryToken();
<script>
	const csrf = document.querySelector("input[name=__RequestVerificationToken]").value;
	const res = await fetch('/api/something', { 
		headers: {
			RequestVerificationToken: csrf
		} 
	});
</script>
```

## Pure token

```cs
var csrf = Antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
```

The above will generate *just* the code and store it in the `csrf` variable. Example usage would be

```html
@inject IAntiforgery Antiforgery

@{
	var csrf = Antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
}

<some-component csrf="@csrf"></some-component>
```