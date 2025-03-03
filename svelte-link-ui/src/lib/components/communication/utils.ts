export type hubTokenType = 'chat' 

export async function getHubToken(tokenType : hubTokenType): Promise<string> {
    try {
        const response = await fetch('/app/hubtokens/chat');
        if (response.ok) {
            const result = await response.text();
            return result;
        } else {
            return '';
        }
    } catch (error) {
        return '';
    }
}