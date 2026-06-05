export async function rawHttp<T>(url: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(url, options);

  if (!response.ok) {
    throw new Error(await response.text());
  }

  return response.json();
}
